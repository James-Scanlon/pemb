using System;

namespace Programme.Api.Dto
{
    public class ProgrammeStateDto
    {
        public int ProgrammeId { get; set; }
        public DateTime? MaturityDate { get; set; }
        public decimal AmountOutstanding { get; set; }
        public bool HasPaydowns { get; set; }
    }
}
