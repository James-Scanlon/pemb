namespace Programme.Api.Domain
{
    public class ProgrammeSupplierCodeMapping
    {
        public int ProgrammeId { get; set; }
        public string SupplierCode { get; set; }

        public bool Match { get; set; }
    }
}
