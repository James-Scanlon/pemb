using System;

namespace Programme.ApiClient.Dto;

// It's not clear if all of the properties here are strictly necessary in the request, as some are looked up and replaced within the controller method.

public class EvergreenPositionDataDto
{
    public int ProgrammeId { get; set; }

    public string Currency { get; set; }

    public string ProgrammeName { get; set; }

    public decimal AmountOutstanding { get; set; }

    public bool CanPayDown { get; set; }

    public bool HasAllocation { get; set; }

    public decimal CurrentPayDownValue { get; set; }
        
    public DateTime? CurrentPayDownDate { get; set; }
        
    public string CurrentPayDownPaymentId { get; set; }
        
    public string CurrentPayDownComments { get; set; }

    public string BatchIdentifier { get; set; }
        
    public string BatchAllocationCandidateIdentifier { get; set; }

    public bool HasUnallocatedEvergreenBatch { get; set; }
}