using System;

namespace Programme.Api.Domain;

public class ProgrammeLimit
{
    public int ProgrammeId { get; set; }

    public decimal Limit { get; set; }

    public string Currency { get; set; }

    public BreachAnalysisMethod BreachAnalysisMethod { get; set; }

    public bool IsEvergreen { get; set; }

    public decimal TotalPurchasesInProgrammeCurrency { get; set; }

    public decimal ReserveAmount { get; set; }

    public LimitGroup ParentGroup { get; set; }

    public decimal? ParentConversionRate { get; set; }

    public decimal? TotalPurchasesInGroupCurrency => TotalPurchasesInProgrammeCurrency * ParentConversionRate;

    public decimal? ReserveAmountInGroupCurrency => ReserveAmount * ParentConversionRate;

    public bool IsLimitBreach => CalculateHeadroomBreachStatus();

    public string LimitText => IsLimitBreach ? GetHeadroomBreachReason() : "Within Facility";

    public bool IsStandardBreach => CalculateStandardBreach();

    public bool IsParentBreach => CalculateParentBreach();

    private bool CalculateStandardBreach()
    {
        if (IsEvergreen) return false;
        if (Limit == 0 || ReserveAmount == 0) return false;

        return Limit - TotalPurchasesInProgrammeCurrency < ReserveAmount;
    }

    private bool CalculateParentBreach()
    {
        if (IsEvergreen) return false;
        if (ParentGroup == null || !ReserveAmountInGroupCurrency.HasValue) return false;

        return ParentGroup.Limit - ParentGroup.GroupTotalPurchasePrice < ReserveAmountInGroupCurrency.Value;
    }

    private bool CalculateHeadroomBreachStatus()
    {
        switch (BreachAnalysisMethod)
        {
            case BreachAnalysisMethod.Standard:
                return IsStandardBreach;

            case BreachAnalysisMethod.Parent:
                return IsParentBreach;

            case BreachAnalysisMethod.Combined:
                return IsStandardBreach || IsParentBreach;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private string GetHeadroomBreachReason()
    {
        switch (BreachAnalysisMethod)
        {
            case BreachAnalysisMethod.Standard:
                return IsStandardBreach ? "Standard Breach" : null;

            case BreachAnalysisMethod.Parent:
                return IsParentBreach ? "Parent Breach" : null;

            case BreachAnalysisMethod.Combined:
                if (IsStandardBreach && IsParentBreach) return "Standard & Parent Breach";
                if (IsStandardBreach) return "Standard Breach";
                if (IsParentBreach) return "Parent Breach";
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        return null;
    }
}
