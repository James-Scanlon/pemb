using System;

namespace Programme.Api.Domain;

public class EvergreenPositionData
{
    public int ProgrammeID { get; set; }
    public string Currency { get; set; }

    public string ProgrammeName { get; set; }
        
    public decimal AmountOutstanding { get; set; }

    public bool CanPayDown => AmountOutstanding > 0 && HasAllocation && !HasUnallocatedEvergreenBatch;

    public bool HasAllocation { get; set; }
    public decimal CurrentPayDownValue { get; set; }
    public DateTime? CurrentPayDownDate { get; set; }
    public string CurrentPayDownPaymentId { get; set; }
    public string CurrentPayDownComments{ get; set; }

    public string BatchIdentifier { get; set; }
    public string BatchAllocationCandidateIdentifier { get; set; }

    public bool HasUnallocatedEvergreenBatch { get; set; }
}