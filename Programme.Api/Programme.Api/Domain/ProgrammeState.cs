using System;

namespace Programme.Api.Domain
{
    public class ProgrammeState
    { 
        public int ProgrammeId { get; set; }
        public DateTime? MaturityDate { get; set; }
        public decimal AmountOutstanding { get; set; }
        public bool HasPaydowns { get; set; }
    }
}
