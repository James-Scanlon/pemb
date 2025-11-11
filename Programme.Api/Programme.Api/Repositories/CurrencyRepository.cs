using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Programme.Api.Domain;
using Programme.Api.Repositories.Row;

namespace Programme.Api.Repositories;

public class CurrencyRepository(
    IDatabaseConnection databaseConnection,
    IMapper mapper,
    IMemoryCache memoryCache,
    ILogger<CurrencyRepository> logger)
    : ICurrencyRepository
{
    private readonly IDatabaseConnection _databaseConnection = databaseConnection ?? throw new ArgumentNullException(nameof(databaseConnection));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    private readonly ILogger<CurrencyRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private const string CacheKey = "currency";

    public async Task<Currency> GetByCodeAsync(string code)
    {
        var all = await GetAllAsync();

        return all.Single(x => x.Code.Equals(code, StringComparison.InvariantCultureIgnoreCase));
    }

    private async Task<IReadOnlyCollection<Currency>> GetAllAsync()
    {
        if (_memoryCache.TryGetValue(CacheKey, out IReadOnlyCollection<Currency> cacheEntry)) return cacheEntry;

        try
        {
            await _semaphore.WaitAsync(-1);

            if (_memoryCache.TryGetValue(CacheKey, out cacheEntry)) return cacheEntry;

            _logger.LogInformation("Currency cache missed");

            using var connection = _databaseConnection.GetConnection();

            var results = await connection.QueryAsync<CurrencyRow>("dbo.usp_ReallocationsGetCurrencies",
                commandType: CommandType.StoredProcedure);

            cacheEntry = _mapper.Map<List<Currency>>(results);

            if (cacheEntry != null && cacheEntry.Count != 0)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(12));

                _memoryCache.Set(CacheKey, cacheEntry, cacheEntryOptions);
                return cacheEntry;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting currencies");
        }
        finally
        {
            _semaphore.Release();
        }

        return new List<Currency>();
    }
}