using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ParkSquare.Extensions.Http;
using Pemberton.ApiClient;
using Programme.ApiClient.Dto;

namespace Programme.ApiClient;

public class ProgrammeApiClient : ApiClientBase, IProgrammeApiClient
{
    private readonly ILogger<ProgrammeApiClient> _logger;
    private readonly Uri _baseUri;

    public ProgrammeApiClient(
        IProgrammeApiConfig config,
        ILogger<ProgrammeApiClient> logger,
        IClientSecurity clientSecurity) : base(config.Scope, logger, clientSecurity)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(logger);

        _logger = logger;

        _baseUri = new Uri(config.Server).Concat(config.BaseUrl);
    }

    public async Task<GetProgrammesResponseDto> GetProgrammesAsync()
    {
        _logger.LogInformation("Getting all programmes");

        var uri = _baseUri.Concat("programme");
        var response = await GetAsync<GetProgrammesResponseDto>(uri).ConfigureAwait(false);

        return response;
    }

    public async Task<ProgrammeDto> GetProgrammeByIdAsync(int programmeId)
    {
        _logger.LogInformation("Getting programme by ID");

        var uri = _baseUri.Concat($"programme/GetProgrammeById/{programmeId}");
        var response = await GetAsync<ProgrammeDto>(uri).ConfigureAwait(false);

        return response;
    }

    public async Task<ProgrammeStateDto> GetCurrentProgrammeStateByBatchAsync(string batchId)
    {
        _logger.LogInformation("Getting programme state by ID");

        var uri = _baseUri.Concat($"programme/CurrentProgrammeStateByBatch/{batchId}");
        var response = await GetAsync<ProgrammeStateDto>(uri).ConfigureAwait(false);

        return response;
    }

    public async Task<List<ProgrammeDto>> GetProgrammeStaticAsync()
    {
        _logger.LogInformation("Getting programme static");

        var uri = _baseUri.Concat("programme/GetProgrammeStaticData");
        var response = await GetAsync<List<ProgrammeDto>>(uri).ConfigureAwait(false);

        return response;
    }

    public async Task<List<EvergreenPositionDataDto>> GetEvergreenAssetPositionsAsync()
    {
        _logger.LogInformation("Getting evergreen position data");

        var uri = _baseUri.Concat("programme/GetEvergreenAssetPositions");
        var response = await GetAsync<List<EvergreenPositionDataDto>>(uri).ConfigureAwait(false);

        return response;
    }

    public async Task<bool> ConvertProgrammeToEvergreenAsync(int programmeId)
    {
        _logger.LogInformation("Getting programme state by ID");

        var uri = _baseUri.Concat($"programme/ConvertProgrammeToEvergreen/{programmeId}");
        return await PostAsync<bool>(uri).ConfigureAwait(false);
    }

    public async Task<int> CreateProgrammeDataAsync(ProgrammeDto request)
    {
        _logger.LogInformation("Create Programme Data");

        var uri = _baseUri.Concat($"programme/CreateProgrammeData");
        return await PostAsync<int>(uri, request).ConfigureAwait(false);
    }

    public async Task<int> UpdateProgrammeDataAsync(ProgrammeDto request)
    {
        _logger.LogInformation("Update Programme Data");

        var uri = _baseUri.Concat($"programme/UpdateProgrammeData");
        return await PostAsync<int>(uri, request).ConfigureAwait(false);
    }

    public async Task<List<ProgrammeLimitDto>> ProgrammeHeadroomAsync(ProgrammeHeadroomStatusRequestDto request)
    {
        _logger.LogInformation("Programme headroom data");

        var uri = _baseUri.Concat($"programme/headroom");
        return await PostAsync<List<ProgrammeLimitDto>>(uri, request).ConfigureAwait(false);
    }


    public async Task<List<EntityTypeDto>> GetOriginationEntitiesAsync()
    {
        _logger.LogInformation("Getting Origination Entities");

        var uri = _baseUri.Concat("programme/GetOriginationEntities");

        var response = await GetAsync<List<EntityTypeDto>>(uri).ConfigureAwait(false);
        return response;
    }

    public async Task OriginationEntitiesDeleteAsync(int id)
    {
        _logger.LogInformation("Origination Entities delete");

        var uri = _baseUri.Concat($"programme/OriginationEntitiesDelete/{id}");

        await PostAsync(uri).ConfigureAwait(false);
    }

    public async Task OriginationEntitiesInsertAsync(string name)
    {
        _logger.LogInformation("Origination Entities delete");

        var uri = _baseUri.Concat($"programme/OriginationEntitiesInsert/{name}");

        await PostAsync(uri).ConfigureAwait(false);
    }

    public async Task MapProgrammeOriginationAsync(int originationId, int programmeId)
    {
        _logger.LogInformation("Origination Entities map");

        var uri = _baseUri.Concat($"programme/MapProgrammeOrigination/{originationId}/{programmeId}");

        await PostAsync(uri).ConfigureAwait(false);
    }

    public async Task RemoveMapProgrammeOriginationAsync(int originationId, int programmeId)
    {
        _logger.LogInformation("Origination Programme Remove Mapping");

        var uri = _baseUri.Concat($"programme/RemoveMapProgrammeOrigination/{originationId}/{programmeId}");

        await PostAsync(uri).ConfigureAwait(false);
    }

    public async Task<List<ProgrammeLimitParentDto>> GetProgrammeLimitParentAsync()
    {
        _logger.LogInformation("Getting programme limit parent");

        var uri = _baseUri.Concat("programme/GetProgrammeLimitParent");

        var response = await GetAsync<List<ProgrammeLimitParentDto>>(uri).ConfigureAwait(false);
        return response;
    }

    public async Task<ProgrammeLimitMappingHeadDto> GetProgrammeLimitParentMappingAsync()
    {
        _logger.LogInformation("Getting programme limit mappings");

        var uri = _baseUri.Concat("programme/GetProgrammeLimitParentMapping");

        var response = await GetAsync<ProgrammeLimitMappingHeadDto>(uri).ConfigureAwait(false);
        return response;
    }

    public async Task MapParentLimitMappingAsync(int programmeId, int parentLimitId)
    {
        _logger.LogInformation("Mapping Parent limit");

        var uri = _baseUri.Concat($"programme/MapParentLimitMapping/{programmeId}/{parentLimitId}");
        await PostAsync(uri).ConfigureAwait(false);
    }

    public async Task CreateProgrammeLimitMappingAsync(ProgrammeLimitParentDto programLimitParentDto)
    {
        _logger.LogInformation("Creating programme limit mapping");

        var uri = _baseUri.Concat("programme/CreateProgrammeLimitMapping");
        await PostAsync(uri, programLimitParentDto).ConfigureAwait(false);
    }

    public async Task UpdateProgrammeLimitMappingAsync(ProgrammeLimitParentDto programLimitParentDto)
    {
        _logger.LogInformation("Updating programme limit mapping");

        var uri = _baseUri.Concat("programme/UpdateProgrammeLimitMapping");
        await PostAsync(uri, programLimitParentDto).ConfigureAwait(false);
    }

    public async Task DeleteProgrammeLimitMappingAsync(int programmeLimitParentId)
    {
        _logger.LogInformation("Deleting Parent limit");

        var uri = _baseUri.Concat($"programme/DeleteProgrammeLimitMapping?programmeLimitParentId={programmeLimitParentId}");
        await DeleteAsync(uri).ConfigureAwait(false);
    }

    public async Task UnmapParentLimitMappingAsync(int programmeId, int parentLimitId)
    {
        _logger.LogInformation("Mapping Parent limit");

        var uri = _baseUri.Concat($"programme/UnmapParentLimitMapping/{programmeId}/{parentLimitId}");
        await PostAsync(uri).ConfigureAwait(false);
    }

    public async Task<AutoPaymentDataDto> GetAutoPaymentDataAsync(int programmeId)
    {
        _logger.LogInformation("Getting autopayment data for programme {ProgrammeId}", programmeId);

        var uri = _baseUri.Concat($"programme/GetAutoPaymentData/{programmeId}");

        return await GetAsync<AutoPaymentDataDto>(uri).ConfigureAwait(false);

    }

    public async Task<bool> DeleteProgrammeSupplierAsync(int programmeSupplierCodeId)
    {
        _logger.LogInformation("Delete programme supplier");

        var uri = _baseUri.Concat($"programme/DeleteProgrammeSupplier/{programmeSupplierCodeId}");
        return await PostAsync<bool>(uri).ConfigureAwait(false);
    }

    public async Task<bool> UpsertProgrammeSupplierAsync(ProgrammeSupplierCodeDto programmeSupplierCodeDto)
    {
        _logger.LogInformation("upsert programme supplier");

        var uri = _baseUri.Concat($"programme/UpsertProgrammeSupplier");
        return await PostAsync<bool>(uri, programmeSupplierCodeDto).ConfigureAwait(false);
    }

    public async Task<GetAutoPaymentSettingsResponseDto> CheckProgrammeAutoPaymentSettingsAsync(AutoPaymentSettingsRequestDto requestDto)
    {
        _logger.LogInformation("Check Programme auto payment settings");

        var uri = _baseUri.Concat($"programme/CheckProgrammeAutoPaymentSettings");
        return await PostAsync<GetAutoPaymentSettingsResponseDto>(uri, requestDto).ConfigureAwait(false);
    }
}