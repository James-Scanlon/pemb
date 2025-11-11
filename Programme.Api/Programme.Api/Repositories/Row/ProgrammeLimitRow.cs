using Programme.Api.Domain;

namespace Programme.Api.Repositories.Row;

public class ProgrammeLimitRow
{
    public int ProgrammeId { get; set; }

    public decimal Limit { get; set; }

    public string Currency { get; set; }

    public BreachAnalysisMethod BreachAnalysisMethod { get; set; }
    
    public bool IsEvergreen { get; set; }

    public decimal TotalPurchasePrice { get; set; }

    public decimal ReserveAmount { get; set; }
}