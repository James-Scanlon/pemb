using AutoMapper;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Pemberton.Rates.Api.Client;
using Programme.Api.Domain;
using Programme.Api.Dto;
using Programme.Api.Exceptions;
using Programme.Api.Repositories.Row;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wcf.Accounts.ApiClient;
using Wcf.Accounts.ApiClient.Dto;

namespace Programme.Api.Repositories
{
    public class ProgrammeRepository(
        IDatabaseConnection databaseConnection,
        IMapper mapper,
        IMemoryCache memoryCache,
        ILogger<ProgrammeRepository> logger,
        IAccountsApiClient accountsApiClient,
        IRatesApiClient ratesApiClient,
        INextIdRepository nextIdRepository)
        : IProgrammeRepository
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly IDatabaseConnection _databaseConnection = databaseConnection ?? throw new ArgumentNullException(nameof(databaseConnection));
        private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        private readonly ILogger<ProgrammeRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IAccountsApiClient _accountsApiClient = accountsApiClient ?? throw new ArgumentNullException(nameof(accountsApiClient));

        private const string CacheKey = "programme";
        public async Task<IReadOnlyCollection<Domain.Programme>> GetAllAsync()
        {
            if (_memoryCache.TryGetValue(CacheKey, out IReadOnlyCollection<Domain.Programme> cacheEntry)) return cacheEntry;

            try
            {
                await _semaphore.WaitAsync(-1);

                if (_memoryCache.TryGetValue(CacheKey, out cacheEntry)) return cacheEntry;

                _logger.LogInformation("Programme cache missed");

                using var connection = _databaseConnection.GetConnection();

                var results = await connection.QueryAsync<ProgrammeRow>("dbo.usp_ReallocationsGetProgrammes",
                    commandType: CommandType.StoredProcedure);

                cacheEntry = _mapper.Map<List<Domain.Programme>>(results);

                if (cacheEntry != null && cacheEntry.Count != 0)
                {
                    foreach (var programme in cacheEntry)
                    {
                        //Get this from the accounts api which will need to be written.
                        var programmeResponse = await _accountsApiClient.GetProgrammeAccountsByIdAsync(programme.ProgrammeId);
                        ProgrammeAccountDto programmeAccountDto = programmeResponse.Value;
                        programme.Account = _mapper.Map<ProgrammeAccount>(programmeAccountDto);
                    }

                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _memoryCache.Set(CacheKey, cacheEntry, cacheEntryOptions);
                    return cacheEntry;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting programmes");
            }
            finally
            {
                _semaphore.Release();
            }

            return new List<Domain.Programme>();
        }

        public async Task<Domain.Programme> GetByIdAsync(int programmeId)
        {
            var programmes = await GetAllProgrammeDataAsync();

            return programmes.Single(p => p.ProgrammeId == programmeId);
        }

        public async Task<ProgrammeState> GetCurrentProgrammeStateByBatchAsync(string batchId)
        {
            using var connection = _databaseConnection.GetConnection();
            var result = await connection.QueryFirstAsync<ProgrammeState>("dbo.usp_CurrentBatchPosition_Get",
                new { BatchIdentifier = batchId }, commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IList<Domain.Programme>> GetAllProgrammeDataAsync()
        {
            using var connection = _databaseConnection.GetConnection();

            var items = await connection.QueryAsync<ProgrammeRow>(
                    "[dbo].[usp_ExtractStaticProgrammeData]",
                    commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            var mapped = _mapper.Map<List<Domain.Programme>>(items);

            return mapped;
        }

        public async Task<IReadOnlyCollection<ProgrammeLimit>> GetAllProgrammeLimitsAsync(List<int> programmeIds)
        {
            if (_memoryCache.TryGetValue(CacheKey, out IReadOnlyCollection<ProgrammeLimit> cacheEntry)) return cacheEntry;

            try
            {
                await _semaphore.WaitAsync(-1);

                if (_memoryCache.TryGetValue(CacheKey, out cacheEntry)) return cacheEntry;

                using var connection = databaseConnection.GetConnection();

                var limitRows = await connection.QueryAsync<ProgrammeLimitRow>(
                        "dbo.usp_ProgrammeGetLimits",
                        commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                var parentLimitRows = (await connection.QueryAsync<ParentLimitRow>(
                        "dbo.usp_ProgrammeGetParentLimits",
                        commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false)).ToList();

                var groupMembershipRows = await connection.QueryAsync<GroupMembershipRow>(
                        "dbo.usp_ProgrammeGetParentLimitMembership",
                        commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                var rates = await ratesApiClient.GetLatestFxRatesAsync();
                var programmeLimits = _mapper.Map<List<ProgrammeLimit>>(limitRows);
                var limitGroups = _mapper.Map<List<LimitGroup>>(parentLimitRows);

                var groupedByParent = groupMembershipRows.ToLookup(x => x.ProgrammeLimitParentId);

                foreach (var group in groupedByParent)
                {
                    var match = limitGroups.SingleOrDefault(x => x.Id == group.Key);
                    if (match == null) continue;

                    var members = programmeLimits.Where(x => group.Any(y => y.ProgrammeId == x.ProgrammeId)).ToList();

                    match.Members = members;
                }

                foreach (var programmeLimit in programmeLimits)
                {
                    var group = limitGroups.SingleOrDefault(x =>
                        x.Members != null && x.Members.Any(y => y.ProgrammeId == programmeLimit.ProgrammeId));

                    if (group != null)
                    {
                        programmeLimit.ParentGroup = group;

                        if (programmeLimit.Currency.Equals(group.Currency))
                        {
                            // Same currency as parent group
                            programmeLimit.ParentConversionRate = 1.0M;
                        }
                        else
                        {
                            var rate = rates.SingleOrDefault(x =>
                                x.FromCode.Equals(programmeLimit.Currency, StringComparison.InvariantCultureIgnoreCase) &&
                                x.ToCode.Equals(group.Currency, StringComparison.InvariantCultureIgnoreCase) &&
                                DateTime.UtcNow.Date > x.TradeDate.Date);

                            if (rate == null)
                            {
                                _logger.LogWarning("No FX rate available from {CurrencyFrom} to {CurrencyTo} for today",
                                    programmeLimit.Currency, group.Currency);

                                throw new ProgramLimitException($"No FX rate available from {programmeLimit.Currency} to {group.Currency} for today");
                            }

                            programmeLimit.ParentConversionRate = rate.Rate;
                        }
                    }
                }

                if (programmeLimits.Count != 0)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));

                    _memoryCache.Set(CacheKey, programmeLimits, cacheEntryOptions);
                }

                return programmeLimits.Where(x => programmeIds.Any(y => y == x.ProgrammeId)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting programme limits");
            }
            finally
            {
                _semaphore.Release();
            }

            return [];
        }

        public async Task<IList<EvergreenPositionData>> GetAllEvergreenAssetPositionsAsync()
        {
            using var connection = databaseConnection.GetConnection();

            try
            {
                var items = await connection.QueryAsync<EvergreenPositionData>(
                        "[dbo].[usp_EvergreenAssetPositionsGet]",
                        commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);

                return items.ToList();
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error getting evergreen asset positions");
            }

            return null;
        }

        public async Task<int> InsertAsync(Domain.Programme item)
        {
            var param = new
            {
                item.AccountNumber,
                item.Name,
                item.Limit,
                SecurityMapping = item.BnyMapping,
                item.Comment,
                BranchID = item.BranchId ?? 0,
                item.UpdatedBy,
                item.EnableForAllocation,
                Currency = item.CurrencyIsoCode,
                item.IsBatchCalculationPrincipal,
                item.OriginationEntityId,
                ProgrammeLimitParentId = item.ProgrammeLimitParentId ?? 0,
                item.BreachAnalysisMethod,
                item.Provider,
                item.AutoPaymentEnabled,
                item.RatesFormat,
                item.UseAutoPaymentSingle
            };

            using var connection = _databaseConnection.GetConnection();

            var programmeId = await connection.ExecuteScalarAsync<int>(
                    "[dbo].[usp_InsertProgrammeData]",
                    param,
                    commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            if (programmeId < 1) throw new DbRepositoryException("Error persisting new programme");

            return programmeId;
        }

        public async Task UpdateAsync(Domain.Programme item)
        {
            if (item.ProgrammeId < 1) return;

            var param = new
            {
                ProgrammeID = item.ProgrammeId,
                item.Limit,
                SecurityMapping = item.BnyMapping,
                item.Comment,
                BranchID = item.BranchId ?? 0,
                item.UpdatedBy,
                item.AccountNumber,
                item.EnableForAllocation,
                item.IsBatchCalculationPrincipal,
                item.IsActive,
                item.OriginationEntityId,
                ProgrammeLimitParentId = item.ProgrammeLimitParentId ?? 0,
                item.BreachAnalysisMethod,
                item.Provider,
                item.AutoPaymentEnabled,
                item.RatesFormat,
                item.UseAutoPaymentSingle
            };

            using var connection = _databaseConnection.GetConnection();

            await connection.ExecuteAsync(
                    "[dbo].[usp_UpdateProgrammeData]",
                    param,
                    commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);

            await UpdateProgrammeAccountInfoAsync(item.ProgrammeId)
            .ConfigureAwait(false);
        }

        public async Task<bool> ConvertProgrammeToEvergreen(int programmeId)
        {
            try
            {
                var newBatchId = await nextIdRepository.GetNextIdStringAsync("BATCH")
                    .ConfigureAwait(false);

                var param = new
                {
                    ProgrammeID = programmeId,
                    BatchIdentifier = newBatchId
                };
                using var connection = databaseConnection.GetConnection();


                await connection.ExecuteAsync(
                        "usp_ConvertProgrammeToEvergreen",
                        param,
                        commandType: CommandType.StoredProcedure)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting programme to evergreen");
                return false;
            }

            return true;
        }

        private async Task UpdateProgrammeAccountInfoAsync(int programmeId)
        {
            var param = new
            {
                ProgrammeID = programmeId,
            };

            using var connection = databaseConnection.GetConnection();

            await connection.ExecuteAsync(
                    sql: "[dbo].[usp_AllocationPaymentPendingUpdateProgrammeAccountInfo]",
                    param: param,
                    commandType: CommandType.StoredProcedure)
                .ConfigureAwait(false);
        }
    

    public async Task<List<EntityType>> GetOriginationEntitiesAsync()
        {
            using var connection = _databaseConnection.GetConnection();
            var rows = await connection.QueryAsync<EntityType>(
                "dbo.usp_OriginationEntities_Get",
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return rows.ToList();
        }

        public async Task OriginationEntitiesInsertAsync(string orginationName)
        {
            var param = new
            {
                OriginationName = orginationName
            };

            using var connection = _databaseConnection.GetConnection();
            await connection.ExecuteAsync(
                "dbo.usp_OriginationEntities_Insert", param: param,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        }

        public async Task OriginationEntitiesDeleteAsync(int originationId)
        {
            var param = new
            {
                OriginationID = originationId
            };

            using var connection = _databaseConnection.GetConnection();
            await connection.ExecuteAsync(
                "dbo.usp_OriginationEntities_Delete", param: param,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        }

        public async Task MapProgrammeOriginationAsync(int? originationId, int? programmeId)
        {
            var param = new
            {
                OriginationID = originationId,
                ProgrammeID = programmeId
            };

            using var connection = _databaseConnection.GetConnection();
            await connection.ExecuteAsync(
                "dbo.usp_ProgrammeOrigination_Mapping", param: param,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        }

        public async Task RemoveMapProgrammeOriginationAsync(int? originationId, int? programmeId)
        {
            var param = new
            {
                OriginationID = originationId,
                ProgrammeID = programmeId
            };

            using var connection = _databaseConnection.GetConnection();
            await connection.ExecuteAsync(
                "dbo.usp_ProgrammeOriginationMapping_Remove", param: param,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        }

        public async Task<List<ProgrammeLimitParent>> GetProgrammeLimitParentAsync()
        {
            using var connection = _databaseConnection.GetConnection();
            var rows = await connection.QueryAsync<ProgrammeLimitParent>(
                "dbo.usp_ProgrammeLimitParentGet",
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return rows.ToList();
        }

        public async Task<ProgrammeLimitParentMappingHead> GetProgrammeLimitParentMappingAsync()
        {
            using var connection = _databaseConnection.GetConnection();
            var rows = await connection.QueryMultipleAsync(
                "dbo.usp_ProgrammeLimitParentMappingGet",
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            var mapped = await rows.ReadAsync<ProgrammeLimitParentMapping>();

            var unmapped = await rows.ReadAsync<ProgrammeLimitParentMapping>();

            var mappedHead = new ProgrammeLimitParentMappingHead { Mapped = mapped.ToList(), Unmapped = unmapped.ToList() };

            return mappedHead;
        }

        public async Task<bool> MapParentLimitMappingAsync(int? programmeId, int? parentLimitId)
        {
            var param = new
            {
                ProgrammeID = programmeId,
                ProgrammeLimitParentID = parentLimitId
            };

            try
            {
                using var connection = _databaseConnection.GetConnection();

                await connection.ExecuteAsync(
                    "[dbo].[usp_ProgrammeLimitParentMappingMap]",
                    param,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            }
            catch (SqlException sqlException)
            {
                _logger.LogError(sqlException, "Error mapping parent limits");
                return false;
            }

            return true;
        }

        public async Task<bool> UnMapParentLimitMappingAsync(int? programmeId, int? parentLimitId)
        {
            var param = new
            {
                ProgrammeID = programmeId,
                ProgrammeLimitParentID = parentLimitId
            };

            try
            {
                using var connection = _databaseConnection.GetConnection();

                await connection.ExecuteAsync(
                    "[dbo].[usp_ProgrammeLimitParentMappingUnmap]",
                    param,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            }
            catch (SqlException sqlException)
            {
                _logger.LogError(sqlException, "Error unmapping parent limits");
                return false;
            }

            return true;
        }

        public async Task<bool> CreateProgrammeLimitMappingAsync(ProgrammeLimitParent domainObject)
        {
            var param = new
            {
                domainObject.ProgrammeLimitParentName,
                domainObject.GroupLimit,
                domainObject.Currency
            };

            try
            {
                using var connection = _databaseConnection.GetConnection();
                await connection.ExecuteAsync(
                    "dbo.usp_ProgrammeLimitParentInsert",
                    param,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            }
            catch (SqlException sqlException)
            {
                _logger.LogError(sqlException, "Error creating programme limit mapping");
                return false;
            }

            return true;
        }


        public async Task<bool> UpdateProgrammeLimitMappingAsync(ProgrammeLimitParent domainObject)
        {
            var param = new
            {
                ProgrammeLimitParentID = domainObject.ProgrammeLimitParentId,
                domainObject.ProgrammeLimitParentName,
                domainObject.GroupLimit,
                domainObject.Currency
            };

            try
            {
                using var connection = _databaseConnection.GetConnection();
                await connection.ExecuteAsync(
                    "dbo.usp_ProgrammeLimitParentEdit",
                    param,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            }
            catch (SqlException sqlException)
            {
                _logger.LogError(sqlException, "Error updating programme limit mapping");
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteProgrammeLimitMappingAsync(int programmeLimitParentId)
        {
            var param = new
            {
                ProgrammeLimitParentID = programmeLimitParentId
            };

            try
            {
                using var connection = _databaseConnection.GetConnection();
                await connection.ExecuteAsync(
                    "dbo.usp_ProgrammeLimitParentDelete",
                    param,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            }
            catch (SqlException sqlException)
            {
                _logger.LogError(sqlException, "Error deleting programme limit mapping");
                return false;
            }

            return true;
        }

        public async Task<AutoPaymentData> GetAutoPaymentDataAsync(int programmeId)
        {
            var param = new
            {
                ProgrammeID = programmeId
            };
            using var connection = _databaseConnection.GetConnection();

            var multipleRows = await connection.QueryMultipleAsync(
                "dbo.usp_GetAutoPaymentData", param: param,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            var canProcesAutoPayments = await multipleRows.ReadFirstAsync<bool>();
            var rows = await multipleRows.ReadAsync<ProgrammeSupplierCode>();

            var autoPaymentData = new AutoPaymentData
            {
                IsAutoPaymentEnabled = canProcesAutoPayments,
                ProgrammeSupplierCodes = rows.ToList()
            };

            return autoPaymentData;

        }

        public async Task<bool> DeleteProgrammeSupplierAsync(int programmeSupplierCodeId)
        {
            var param = new
            {
                ID = programmeSupplierCodeId
            };

            try
            {
                using var connection = _databaseConnection.GetConnection();
                await connection.ExecuteAsync(
                    "dbo.usp_ProgrammeSupplierCode_Delete",
                    param,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            }
            catch (SqlException sqlException)
            {
                _logger.LogError(sqlException, "Error deleting programme supplier");
                return false;
            }

            return true;
        }

        public async Task<bool> UpsertProgrammeSupplierAsync(Dto.ProgrammeSupplierCodeDto programmeSuppliers)
        {
            var param = new
            {
                ID = programmeSuppliers.ProgrammeSupplierCodeId,
                programmeSuppliers.SupplierCode,
                ProgrammeID = programmeSuppliers.ProgrammeId
            };

            try
            {
                using var connection = _databaseConnection.GetConnection();

                var storedProcedure = programmeSuppliers.ProgrammeSupplierCodeId > 0
                    ? "dbo.usp_ProgrammeSupplierCode_Update"
                    : "dbo.usp_ProgrammeSupplierCode_Insert";

                await connection.ExecuteAsync(
                    storedProcedure,
                    param,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            }
            catch (SqlException sqlException)
            {
                _logger.LogError(sqlException, "Error inserting/updating programme supplier");
                return false;
            }

            return true;
        }

        public async Task<AutoPaymentParent> AutoPaymentSettingsGetAsync(AutoPaymentSettingsRequest autopaymentSettingsRequest)
        {
            var autopaymentSettingsTable = BuildAutoPaymentRequest(autopaymentSettingsRequest);

            var param = new DynamicParameters(new
            {
                AutoPaymentSettings = autopaymentSettingsTable.AsTableValuedParameter("AutoPaymentSettings"),
                
            });
            
            using var connection = _databaseConnection.GetConnection();

            var autoPaymentSettingMultiple = await connection.QueryMultipleAsync(
              "dbo.usp_AutoPaymentSettings_Get", param: param,
              commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            var result = await autoPaymentSettingMultiple.ReadFirstAsync<AutoPaymentCheck>();


            var autoPaymentSetting = await autoPaymentSettingMultiple.ReadAsync<AutoPaymentSetting>();

            foreach (var setting in autoPaymentSetting)
            {
                setting.HasProgrammeSupplierMapping = string.IsNullOrEmpty(setting.SupplierCode) ? false : true;
            }

            var autoPaymentParent = new AutoPaymentParent();

            autoPaymentParent.AutoPaymentEnabled = result.AutoPaymentEnabled;

            autoPaymentParent.Setting = autoPaymentSetting;

            return autoPaymentParent;
        }

        private DataTable BuildAutoPaymentRequest(AutoPaymentSettingsRequest uploadTradeFormat)
        {
            var dt = new DataTable();
            dt.Columns.Add("ProgrammeID", typeof(int));
            dt.Columns.Add("SupplierCode", typeof(string));
            

            foreach (var data in uploadTradeFormat.SupplierCodes)
            {
                var row = dt.NewRow();
                row["ProgrammeID"] = uploadTradeFormat.ProgrammeId;
                row["SupplierCode"] = data;

                dt.Rows.Add(row);
            }

            return dt;
        }
    }
}