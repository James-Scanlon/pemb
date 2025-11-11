using System.Collections.Generic;

namespace Programme.Api.Domain
{
    public class AutoPaymentData
    {
        public bool IsAutoPaymentEnabled { get; set; }
        public List<ProgrammeSupplierCode> ProgrammeSupplierCodes { get; set; }
    }
}
