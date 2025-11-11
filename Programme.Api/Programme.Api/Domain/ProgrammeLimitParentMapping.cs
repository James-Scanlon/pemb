namespace Programme.Api.Domain
{
    public class ProgrammeLimitParentMapping
    {
        public int ProgrammeLimitParentMappingId { get; set; }
        public int ProgrammeId { get; set; }

        public string ProgrammeName { get; set; }

        public int ProgrammeLimitParentId { get; set; }
    }
}
