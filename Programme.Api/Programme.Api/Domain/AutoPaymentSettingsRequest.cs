using System.Collections.Generic;

namespace Programme.Api.Domain
{
    public class AutoPaymentSettingsRequest
    {
        public int ProgrammeId { get; set; }
        public List<string> SupplierCodes { get; set; }
    }
}
