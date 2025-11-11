using Programme.Api.Domain;
using Programme.Api.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Programme.Api.Repositories
{
    public interface IProgrammeRepository
    {
        Task<IReadOnlyCollection<Domain.Programme>> GetAllAsync();
        Task<Domain.Programme> GetByIdAsync(int programmeId);
        Task<Domain.ProgrammeState> GetCurrentProgrammeStateByBatchAsync(string batchId);

        Task<IList<Domain.Programme>> GetAllProgrammeDataAsync();

        Task<IReadOnlyCollection<ProgrammeLimit>> GetAllProgrammeLimitsAsync(List<int> programmeIds);

        Task<IList<EvergreenPositionData>> GetAllEvergreenAssetPositionsAsync();

        Task<bool> ConvertProgrammeToEvergreen(int programmeId);

        Task<int> InsertAsync(Domain.Programme item);

        Task UpdateAsync(Domain.Programme item);

        Task<List<EntityType>> GetOriginationEntitiesAsync();
      

        Task OriginationEntitiesInsertAsync(string orginationName);
      

        Task OriginationEntitiesDeleteAsync(int originationId);
       

        Task MapProgrammeOriginationAsync(int? originationId, int? programmeId);
       

        Task RemoveMapProgrammeOriginationAsync(int? originationId, int? programmeId);
       

        Task<List<ProgrammeLimitParent>> GetProgrammeLimitParentAsync();
     

        Task<ProgrammeLimitParentMappingHead> GetProgrammeLimitParentMappingAsync();
     
        Task<bool> MapParentLimitMappingAsync(int? programmeId, int? parentLimitId);
      

        Task<bool> UnMapParentLimitMappingAsync(int? programmeId, int? parentLimitId);
       
        Task<bool> CreateProgrammeLimitMappingAsync(ProgrammeLimitParent domainObject);
      
        Task<bool> UpdateProgrammeLimitMappingAsync(ProgrammeLimitParent domainObject);
        

        Task<bool> DeleteProgrammeLimitMappingAsync(int programmeLimitParentId);
        
        Task<AutoPaymentData> GetAutoPaymentDataAsync(int programmeId);
       

        Task<bool> DeleteProgrammeSupplierAsync(int programmeSupplierCodeId);
       

        Task<bool> UpsertProgrammeSupplierAsync(ProgrammeSupplierCodeDto programmeSuppliers);

        Task<AutoPaymentParent> AutoPaymentSettingsGetAsync(AutoPaymentSettingsRequest autopaymentSettingsRequest);
    }
}
