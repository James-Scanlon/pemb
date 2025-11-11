namespace Programme.ApiClient.Dto
{
    public class MappedProgrammeSupplierCodeDto
    {
        public bool AutoPaymentEnabled { get; set; }
        public string SupplierCode { get; set; }

        public bool HasProgrammeSupplierMapping { get; set; }

        public bool HasAccountMapping { get; set; }

        public bool UseAutoPaymentSingle { get; set; }
    }
}
