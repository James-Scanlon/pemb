namespace Programme.Api.Repositories.Row;

public class ProgrammeSupplierAccountRow
{
    public int ProgrammeId { get; set; }

    public int BranchId { get; set; }

    public string AccountNumber { get; set; }

    public string Currency { get; set; }

    public int BnyBranchId { get; set; }

    public string Name { get; set; }

    public int EntityTypeId { get; set; }
}