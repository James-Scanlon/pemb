using System.Collections;
using System.Collections.Generic;

namespace Programme.Api.Domain
{
    public class AutoPaymentParent
    {
        public IEnumerable<AutoPaymentSetting> Setting { get; set; }

        public bool AutoPaymentEnabled { get; set; }
    }
}
