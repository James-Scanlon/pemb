namespace Programme.ApiClient.Dto
{
    public class GetAutoPaymentSettingsResponseDto
    {
        public bool IsSuccess { get; internal set; }
        public bool AutoPaymentEnabled { get; set; }
        public List<MappedProgrammeSupplierCodeDto> MappedSupplierCodes { get; set; }
    }
}
