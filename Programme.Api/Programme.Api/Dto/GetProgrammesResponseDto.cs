using System.Collections.Generic;

namespace Programme.Api.Dto;

public class GetProgrammesResponseDto
{
    public bool IsSuccess { get; set; }

    public IReadOnlyCollection<ProgrammeDto> Value { get; set; }
}