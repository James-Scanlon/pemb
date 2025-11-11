using System;
using System.Collections.Generic;

namespace Programme.Api.Domain
{
    public class ProgrammeLimitParentMappingHead
    {
        public List<ProgrammeLimitParentMapping> Mapped { get; set; } = new();
        public List<ProgrammeLimitParentMapping> Unmapped { get; set; } = new();

        
    }
}
