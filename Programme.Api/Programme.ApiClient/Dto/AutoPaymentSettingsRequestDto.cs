namespace Programme.ApiClient.Dto
{
    public class AutoPaymentSettingsRequestDto
    {
        public int ProgrammeId { get; set; }
        public List<string> SupplierCodes { get; set; }
    }
}
