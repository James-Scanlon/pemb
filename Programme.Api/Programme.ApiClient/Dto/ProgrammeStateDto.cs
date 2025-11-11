using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programme.ApiClient.Dto
{
    public class ProgrammeStateDto
    {
        public int ProgrammeId { get; set; }
        public DateTime? MaturityDate { get; set; }
        public decimal AmountOutstanding { get; set; }
        public bool HasPaydowns { get; set; }
    }
}
