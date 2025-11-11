using Asp.Versioning;
using Audit.ApiClient;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Programme.Api.Domain;
using Programme.Api.Dto;
using Programme.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Programme.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class ProgrammeController(IMapper mapper, 
    IProgrammeRepository programmeRepository, 
    ILogger<ProgrammeController> logger,
    IAuditApiClient auditClient) : ControllerBase
{
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IProgrammeRepository _programmeRepository = programmeRepository ?? throw new ArgumentNullException(nameof(programmeRepository));

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var programmes = await programmeRepository.GetAllAsync();

        if (!programmes.Any()) return NotFound();

        var response = new GetProgrammesResponseDto
        {
            IsSuccess = true,
            Value = _mapper.Map<IReadOnlyCollection<ProgrammeDto>>(programmes)
        };

        return Ok(response);
    }

    [HttpPost("CheckProgrammeAutoPaymentSettings")]

    public async Task<IActionResult> CheckProgrammeAutoPaymentSettingsAsync(AutoPaymentSettingsRequestDto autopaymentSettingsRequestDto)
    {
        var autopaymentSettingsRequest = mapper.Map<AutoPaymentSettingsRequest>(autopaymentSettingsRequestDto);
        var autoPaymentSetting = await programmeRepository.AutoPaymentSettingsGetAsync(autopaymentSettingsRequest);

        var response = new GetAutoPaymentSettingsResponseDto
        {
            IsSuccess = true,
            AutoPaymentEnabled = autoPaymentSetting.AutoPaymentEnabled,
            MappedSupplierCodes = mapper.Map<List<MappedProgrammeSupplierCodeDto>>(autoPaymentSetting.Setting)
        };

        return Ok(response);
    }

    [HttpGet("GetProgrammeById/{programmeId}")]
    public async Task<IActionResult> GetByIdAsync(int programmeId)
    {
        var programme = await programmeRepository.GetByIdAsync(programmeId);
        var response =  _mapper.Map<ProgrammeDto>(programme);
        return Ok(response);
    }

    [HttpGet("CurrentProgrammeStateByBatch/{batchId}")]
    public async Task<IActionResult> GetCurrentProgrammeStateByBatchAsync(string batchId)
    {
        var programmeState = await programmeRepository.GetCurrentProgrammeStateByBatchAsync(batchId);
        var response = _mapper.Map<ProgrammeStateDto>(programmeState);
        return Ok(response);
    }

    [HttpGet("GetProgrammeStaticData")]
    public async Task<IActionResult> GetProgrammeStatic()
    {
        try
        {
            var result = await programmeRepository.GetAllProgrammeDataAsync().ConfigureAwait(false);
            var response = _mapper.Map<List<ProgrammeDto>>(result);

            return Ok(response);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to retrieve static programme data");
            return Problem(e.Message, statusCode: 500);
        }
    }

    [HttpGet("GetEvergreenAssetPositions")]
    public async Task<IActionResult> GetEvergreenAssetPositions()
    {
        try
        {
            var result = await _programmeRepository.GetAllEvergreenAssetPositionsAsync().ConfigureAwait(false);
            var response = _mapper.Map<List<EvergreenPositionDataDto>>(result);

            return Ok(response);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to retrieve evergreen asset positions");
            return Problem(e.Message, statusCode: 500);
        }
    }

    [HttpPost("ConvertProgrammeToEvergreen/{programmeId}")]
    public async Task<IActionResult> ConvertProgrammeToEvergreen(int programmeId)
    {
        try
        {
            var result = await _programmeRepository.ConvertProgrammeToEvergreen(programmeId).ConfigureAwait(false);
            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error converting programme to Evergreen");
            return Problem(e.Message, statusCode: 500);
        }
        finally
        {
            await auditClient.AuditAsync("Programme.Api", "ConvertProgrammeToEvergreen", new Dictionary<string, string>
            {
                { "ProgrammeId", programmeId.ToString() }
            }).ConfigureAwait(false);
        }
    }

    [HttpPost("CreateProgrammeData")]
    public async Task<IActionResult> CreateProgrammeData([FromBody] ProgrammeDto request)
    {
        try
        {
            var programmeData = _mapper.Map<Domain.Programme>(request);
            var id = await _programmeRepository.InsertAsync(programmeData).ConfigureAwait(false);

            return Created($"/programme/edit?programmeId={id}", id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating programme data");
            return Problem(ex.Message, statusCode: 500);
        }
        finally
        {
            await auditClient.AuditAsync("Programme.Api", "CreateProgrammeData", new Dictionary<string, string>
            {
                { "ProgrammeName", request.Name },
                { "AccountNumber", request.AccountNumber },
                { "Limit", request.Limit.ToString() },
                { "Currency", request.CurrencyIsoCode },
                { "IsEvergreen", request.IsEvergreen.ToString() },
                { "Provider", request.Provider },
                { "EnableForAllocation", request.EnableForAllocation.ToString() },
                { "IsBatchCalculationPrincipal", request.IsBatchCalculationPrincipal.ToString() },
                { "Programme ID", request.ProgrammeId.ToString() },
                { "Programme Limit Parent ID", request.ProgrammeLimitParentId.ToString() },
                { "Is Active", request.IsActive.ToString() },
                { "Origination Entity ID", request.OriginationEntityId.ToString() },
                { "Grouped Limit", request.GroupedLimit.ToString() },
                { "Autopayment Enabled", request.AutoPaymentEnabled.ToString() },
                { "Rates Format", request.RatesFormat.ToString() },
                { "AutoPayment Single", request.UseAutoPaymentSingle.ToString() }

            }).ConfigureAwait(false);
        }
    }

    [HttpPost("UpdateProgrammeData")]
    public async Task<IActionResult> UpdateProgrammeData([FromBody] ProgrammeDto request)
    {
        try
        {
            var programmeData = _mapper.Map<Domain.Programme>(request);
            await _programmeRepository.UpdateAsync(programmeData).ConfigureAwait(false);

            return Ok(programmeData.ProgrammeId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating programme data");
            return Problem(ex.Message, statusCode: 500);
        }
        finally
        {
            await auditClient.AuditAsync("Programme.Api", "Update Programme Data", new Dictionary<string, string>
            {
                { "ProgrammeName", request.Name },
                { "AccountNumber", request.AccountNumber },
                { "Limit", request.Limit.ToString() },
                { "Currency", request.CurrencyIsoCode },
                { "IsEvergreen", request.IsEvergreen.ToString() },
                { "Provider", request.Provider },
                { "EnableForAllocation", request.EnableForAllocation.ToString() },
                { "IsBatchCalculationPrincipal", request.IsBatchCalculationPrincipal.ToString() },
                { "Programme ID", request.ProgrammeId.ToString() },
                { "Programme Limit Parent ID", request.ProgrammeLimitParentId.ToString() },
                { "Is Active", request.IsActive.ToString() },
                { "Origination Entity ID", request.OriginationEntityId.ToString() },
                { "Grouped Limit", request.GroupedLimit.ToString() },
                { "Autopayment Enabled", request.AutoPaymentEnabled.ToString() },
                { "Rates Format", request.RatesFormat.ToString() },
                { "AutoPayment Single", request.UseAutoPaymentSingle.ToString() }

            }).ConfigureAwait(false);
        }

    }

    [HttpPost("headroom")]
    public async Task<IActionResult> ProgrammeHeadroomAsync(ProgrammeHeadroomStatusRequestDto request)
    {
        var programmeLimits = await _programmeRepository.GetAllProgrammeLimitsAsync(request.ProgrammeIds);

        var mapped = _mapper.Map<List<ProgrammeLimitDto>>(programmeLimits);

        return Ok(mapped);
    }

    [HttpGet("GetOriginationEntities")]
    public async Task<IActionResult> GetOriginationEntitiesAsync()
    {
        var originationEntities = await _programmeRepository.GetOriginationEntitiesAsync();

        var originationEntitiesDto = mapper.Map<List<EntityTypeDto>>(originationEntities);

        return Ok(originationEntitiesDto);
    }

    [HttpGet("GetProgrammeLimitParent")]
    public async Task<IActionResult> GetProgrammeLimitParentAsync()
    {
        var programmeLimitParents = await _programmeRepository.GetProgrammeLimitParentAsync();

        var programmeLimitParentsDto = mapper.Map<List<ProgrammeLimitParentDto>>(programmeLimitParents);

        return Ok(programmeLimitParentsDto);
    }

    [HttpPost("CreateProgrammeLimitMapping")]
    public async Task<IActionResult> CreateProgrammeLimitMapping(ProgrammeLimitParentDto programLimitParentDto)
    {
        var domainObject = mapper.Map<ProgrammeLimitParent>(programLimitParentDto);
        var result = await _programmeRepository.CreateProgrammeLimitMappingAsync(domainObject);
        return Ok(result);
    }

    [HttpPost("UpdateProgrammeLimitMapping")]
    public async Task<IActionResult> UpdateProgrammeLimitMapping(ProgrammeLimitParentDto programLimitParentDto)
    {
        var domainObject = mapper.Map<ProgrammeLimitParent>(programLimitParentDto);
        var result = await _programmeRepository.UpdateProgrammeLimitMappingAsync(domainObject);
        return Ok(result);
    }

    [HttpGet("GetProgrammeLimitParentMapping")]
    public async Task<IActionResult> GetProgrammeLimitParentMappingAsync()
    {
        var programmeLimitParentMappings = await _programmeRepository.GetProgrammeLimitParentMappingAsync();

        var programmeLimitParentMappingsDto = mapper.Map<ProgrammeLimitMappingHeadDto>(programmeLimitParentMappings);

        return Ok(programmeLimitParentMappingsDto);
    }

    [HttpPost("MapParentLimitMapping/{programmeId}/{parentLimitId}")]
    public async Task<IActionResult> MapParentLimitMappingAsync(int? programmeId, int? parentLimitId)
    {
        var result = await _programmeRepository.MapParentLimitMappingAsync(programmeId, parentLimitId);
        return Ok(result);
    }

    [HttpPost("UnmapParentLimitMapping/{programmeId}/{parentLimitId}")]
    public async Task<IActionResult> UnMapParentLimitMappingAsync(int? programmeId, int? parentLimitId)
    {
        var result = await _programmeRepository.UnMapParentLimitMappingAsync(programmeId, parentLimitId);
        return Ok(result);
    }

    [HttpPost("OriginationEntitiesDelete/{id}")]
    public async Task<IActionResult> OriginationEntitiesDeleteAsync(int id)
    {
        try
        {

            await _programmeRepository.OriginationEntitiesDeleteAsync(id).ConfigureAwait(false);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error modifying data");
            return Problem(e.Message, statusCode: 500);
        }
        finally
        {
            await auditClient.AuditAsync("Programme.Api", "Programme Origination Delete", new Dictionary<string, string>
            {
                { "ID", id.ToString() }
            }).ConfigureAwait(false);
        }
    }

    [HttpPost("OriginationEntitiesInsert/{name}")]
    public async Task<IActionResult> OriginationEntitiesInsertAsync(string name)
    {
        try
        {

            await _programmeRepository.OriginationEntitiesInsertAsync(name).ConfigureAwait(false);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error modifying data");
            return Problem(e.Message, statusCode: 500);
        }
        finally
        {
            await auditClient.AuditAsync("Programme.Api", "Origination Entities insert Async", new Dictionary<string, string>
            {
                { "Name", name.ToString() }
            }).ConfigureAwait(false);
        }
    }

    [HttpPost("MapProgrammeOrigination/{originationId}/{programmeId}")]
    public async Task<IActionResult> MapProgrammeOriginationAsync(int? originationId, int? programmeId)
    {
        try
        {

            await _programmeRepository.MapProgrammeOriginationAsync(originationId, programmeId).ConfigureAwait(false);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error modifying data");
            return Problem(e.Message, statusCode: 500);
        }
        finally
        {
            await auditClient.AuditAsync("Programme.Api", "Map Programme Origination Async", new Dictionary<string, string>
            {
                { "Origination ID", originationId.ToString() },
                { "Programme ID", programmeId.ToString() }
            }).ConfigureAwait(false);
        }
    }

    [HttpPost("RemoveMapProgrammeOrigination/{originationId}/{programmeId}")]
    public async Task<IActionResult> RemoveMapProgrammeOriginationAsync(int? originationId, int? programmeId)
    {
        try
        {

            await _programmeRepository.RemoveMapProgrammeOriginationAsync(originationId, programmeId).ConfigureAwait(false);
            return Ok();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error modifying data");
            return Problem(e.Message, statusCode: 500);
        }
        finally
        {
            await auditClient.AuditAsync("Programme.Api", "Remove Map Programme Origination", new Dictionary<string, string>
            {
                { "Origination ID", originationId.ToString() },
                { "Programme ID", programmeId.ToString() }
            }).ConfigureAwait(false);
        }
    }

    [HttpDelete("DeleteProgrammeLimitMapping")]
    public async Task<IActionResult> DeleteProgrammeLimitMapping(int programmeLimitParentId)
    {
        var result = await _programmeRepository.DeleteProgrammeLimitMappingAsync(programmeLimitParentId);
        return Ok(result);
    }

    [HttpPost("DeleteProgrammeSupplier/{programmeSupplierCodeId}")]
    public async Task<IActionResult> DeleteProgrammeSupplierAsync(int programmeSupplierCodeId)
    {
        try
        {
            var result = await _programmeRepository.DeleteProgrammeSupplierAsync(programmeSupplierCodeId).ConfigureAwait(false);
            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error modifying data");
            return Problem(e.Message, statusCode: 500);
        }
        finally
        {
            await auditClient.AuditAsync("Programme.Api", "Delete Programme Supplier", new Dictionary<string, string>
            {
                { "Programme Supplier Code ID", programmeSupplierCodeId.ToString() }
            }).ConfigureAwait(false);

        }
    }

    [HttpPost("UpsertProgrammeSupplier")]
    public async Task<IActionResult> UpsertProgrammeSupplierAsync(ProgrammeSupplierCodeDto programmeSuppliers)
    {
        try
        {

            var result = await _programmeRepository.UpsertProgrammeSupplierAsync(programmeSuppliers).ConfigureAwait(false);
            return Ok(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error modifying data");
            return Problem(e.Message, statusCode: 500);
        }
        finally
        {
            await auditClient.AuditAsync("Programme.Api", "Upsert Programme Supplier", new Dictionary<string, string>
        {
            { "Programme ID", programmeSuppliers.ProgrammeId.ToString() },
            { "Programme Supplier ID", programmeSuppliers.ProgrammeSupplierCodeId.ToString() },
            { "Supplier Code", programmeSuppliers.SupplierCode.ToString() },
            { "Account ID", programmeSuppliers.SwiftBnyAccountId.ToString() },
        }).ConfigureAwait(false);
        }
    }

    [HttpGet("GetAutoPaymentData/{programmeId}")]
    public async Task<IActionResult> GetAutoPaymentDataAsync(int programmeId)
    {
        var autoPaymentData = await _programmeRepository.GetAutoPaymentDataAsync(programmeId);

        var response = _mapper.Map<AutoPaymentDataDto>(autoPaymentData);
       return Ok(response);
    }



}
