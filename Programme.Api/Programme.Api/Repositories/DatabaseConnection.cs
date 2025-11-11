using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Pemberton.KeyVault;

namespace Programme.Api.Repositories;

public class DatabaseConnection : IDatabaseConnection
{
    private readonly string _connectionString;


    public DatabaseConnection(IDatabaseConfiguration config, IKeyVaultService keyVaultService)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));
        if (keyVaultService == null) throw new ArgumentNullException(nameof(keyVaultService));

        _connectionString = keyVaultService.GetVaultSecret(config.SqlConnectionKey);
    }

    public IDbConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }
}