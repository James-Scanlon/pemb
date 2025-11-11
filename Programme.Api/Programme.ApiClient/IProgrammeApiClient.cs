using Microsoft.AspNetCore.Mvc;
using Programme.ApiClient.Dto;

namespace Programme.ApiClient;

public interface IProgrammeApiClient
{
    Task<GetProgrammesResponseDto> GetProgrammesAsync();
    Task<ProgrammeDto> GetProgrammeByIdAsync(int programmeId);

    Task<ProgrammeStateDto> GetCurrentProgrammeStateByBatchAsync(string batchId);

    Task<List<ProgrammeDto>> GetProgrammeStaticAsync();

    Task<List<EvergreenPositionDataDto>> GetEvergreenAssetPositionsAsync();

    Task<bool> ConvertProgrammeToEvergreenAsync(int programmeId);

    Task<int> CreateProgrammeDataAsync(ProgrammeDto request);

    Task<int> UpdateProgrammeDataAsync(ProgrammeDto request);

    Task<List<ProgrammeLimitDto>> ProgrammeHeadroomAsync(ProgrammeHeadroomStatusRequestDto request);

    Task<List<EntityTypeDto>> GetOriginationEntitiesAsync();

    Task<List<ProgrammeLimitParentDto>> GetProgrammeLimitParentAsync();

    Task<ProgrammeLimitMappingHeadDto> GetProgrammeLimitParentMappingAsync();

    Task MapParentLimitMappingAsync(int programmeId, int parentLimitId);

    Task UnmapParentLimitMappingAsync(int programmeId, int parentLimitId);

    Task OriginationEntitiesDeleteAsync(int id);

    Task OriginationEntitiesInsertAsync(string name);

    Task MapProgrammeOriginationAsync(int originationId, int programmeId);

    Task RemoveMapProgrammeOriginationAsync(int originationId, int programmeId);

    Task CreateProgrammeLimitMappingAsync(ProgrammeLimitParentDto programLimitParentDto);

    Task UpdateProgrammeLimitMappingAsync(ProgrammeLimitParentDto programLimitParentDto);

    Task DeleteProgrammeLimitMappingAsync(int programmeLimitParentId);

   

    Task<AutoPaymentDataDto> GetAutoPaymentDataAsync(int programmeId);

    Task<bool> DeleteProgrammeSupplierAsync(int programmeSupplierCodeId);

    Task<bool> UpsertProgrammeSupplierAsync(ProgrammeSupplierCodeDto programmeSupplierCodeDto);

    Task<GetAutoPaymentSettingsResponseDto> CheckProgrammeAutoPaymentSettingsAsync(AutoPaymentSettingsRequestDto requestDto);


}