using System.Data;

namespace Programme.Api.Repositories;

public interface IDatabaseConnection
{
    IDbConnection GetConnection();
}