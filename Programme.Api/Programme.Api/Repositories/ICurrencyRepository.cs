using System.Threading.Tasks;
using Programme.Api.Domain;

namespace Programme.Api.Repositories;

public interface ICurrencyRepository
{
    Task<Currency> GetByCodeAsync(string code);
}