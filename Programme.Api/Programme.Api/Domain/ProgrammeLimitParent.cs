namespace Programme.Api.Domain;

public class ProgrammeLimitParent
{
    public int ProgrammeLimitParentId { get; set; }

    public string ProgrammeLimitParentName { get; set; }

    public string Currency { get; set; }

    public decimal GroupLimit { get; set; }
}