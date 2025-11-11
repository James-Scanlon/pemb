namespace Programme.Api.Dto
{
    public class ProgrammeLimitDto
    {
        public int ProgrammeId { get; set; }

        public bool IsLimitBreach { get; set; }

        public string LimitText { get; set; }
    }
}
