using System.Threading.Tasks;

namespace Programme.Api.Repositories;

public interface INextIdRepository
{ 
    Task<string> GetNextIdStringAsync(string name);
}