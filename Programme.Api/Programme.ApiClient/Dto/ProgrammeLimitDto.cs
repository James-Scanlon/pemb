using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programme.ApiClient.Dto
{
    public class ProgrammeLimitDto
    {
        public int ProgrammeId { get; set; }

        public bool IsLimitBreach { get; set; }

        public string LimitText { get; set; }
    }
}
