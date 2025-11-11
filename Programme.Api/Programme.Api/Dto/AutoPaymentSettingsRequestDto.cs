using System.Collections.Generic;

namespace Programme.Api.Dto
{
    public class AutoPaymentSettingsRequestDto
    {
        public int ProgrammeId { get; set; }
        public List<string> SupplierCodes { get; set; }
    }
}
