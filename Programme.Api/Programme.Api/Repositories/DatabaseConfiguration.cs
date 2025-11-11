using System;
using Microsoft.Extensions.Configuration;

namespace Programme.Api.Repositories;

public class DatabaseConfiguration : IDatabaseConfiguration
{
    private readonly IConfiguration _configuration;

    public DatabaseConfiguration(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public string SqlConnectionKey => _configuration["KeyVaultSqlConnection"];
}