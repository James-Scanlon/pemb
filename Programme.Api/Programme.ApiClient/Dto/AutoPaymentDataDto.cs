using System.Collections.Generic;

namespace Programme.ApiClient.Dto
{
    public class AutoPaymentDataDto
    {
        public bool IsAutoPaymentEnabled { get; set; }
        public List<ProgrammeSupplierCodeDto> ProgrammeSupplierCodes { get; set; }
    }
}
