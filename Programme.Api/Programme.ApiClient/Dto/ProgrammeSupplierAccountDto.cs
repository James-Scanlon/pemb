namespace Programme.ApiClient.Dto;

public class ProgrammeSupplierAccountDto
{
    public int ProgrammeId { get; set; }

    public int BranchId { get; set; }

    public string Name { get; set; }

    public string Number { get; set; }

    public int BnyBranchId { get; set; }

    public string Currency { get; set; }

    public int EntityTypeId { get; set; }
}