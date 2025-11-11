using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Programme.Api.Repositories;

public class NextIdRepository(IDatabaseConnection databaseConnection) : INextIdRepository
{
    public async Task<string> GetNextIdStringAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
        }

        var nextId = await GetNextIdInternalAsync(name, 1).ConfigureAwait(false);

        return name == "BATCH" ? $"B{nextId}" : $"{name}{nextId}";
    }

    private async Task<long> GetNextIdInternalAsync(string name, int increment)
    {
        var parameters = new DynamicParameters(new
        {
            Name = name,
            Increment = increment,
        });
        parameters.Add(name: "NextID", dbType: DbType.Int64, direction: ParameterDirection.Output);

        using var connection = databaseConnection.GetConnection();

        await connection.ExecuteAsync(
                sql: "[dbo].[GetNextID]",
                param: parameters,
                commandType: CommandType.StoredProcedure)
            .ConfigureAwait(false);

        var nextId = parameters.Get<long>("NextID");

        return nextId;
    }
}