namespace Programme.Api.Repositories.Row;

public class ParentLimitRow
{
    public int ProgrammeId { get; set; }

    public int ProgrammeLimitParentId { get; set; }

    public string ProgrammeLimitParentName { get; set; }

    public decimal GroupLimit { get; set; }

    public string Currency { get; set; }
}