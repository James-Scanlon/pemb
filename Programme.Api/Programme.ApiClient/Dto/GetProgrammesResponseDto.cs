namespace Programme.ApiClient.Dto;

public class GetProgrammesResponseDto
{
    public bool IsSuccess { get; set; }

    public IReadOnlyCollection<ProgrammeDto> Value { get; set; }
}