namespace Programme.ApiClient.Dto
{
    public class ProgrammeSupplierCodeDto
    {
        public int ProgrammeSupplierCodeId { get; set; }
        public string SupplierCode { get; set; }
        public int ProgrammeId { get; set; }

        public int? SwiftBnyAccountId { get; set; }
    }
}
