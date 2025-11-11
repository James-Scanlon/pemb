namespace Programme.Api.Domain
{
    public class ProgrammeSupplierCode
    {
        public int ProgrammeSupplierCodeId { get; set; }
        public string SupplierCode { get; set; }
        public int ProgrammeId { get; set; }

        public int? SwiftBnyAccountId { get; set; }
    }
}
