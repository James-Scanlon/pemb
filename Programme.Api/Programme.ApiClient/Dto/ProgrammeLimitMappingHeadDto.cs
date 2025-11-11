using System.Collections.Generic;

namespace Programme.ApiClient.Dto;

public class ProgrammeLimitMappingHeadDto
{
    public List<ProgrammeLimitParentMappingDto> Mapped { get; set; }

    public List<ProgrammeLimitParentMappingDto> Unmapped { get; set; }
}