using System.Collections.Generic;
using System.Linq;

namespace Programme.Api.Domain;

public class LimitGroup
{
    public int Id { get; set; }

    public string Name { get; set; }

    public decimal Limit { get; set; }

    public string Currency { get; set; }

    public IReadOnlyCollection<ProgrammeLimit> Members { get; set; }

    public decimal GroupTotalPurchasePrice => Members.Sum(x => x.TotalPurchasesInGroupCurrency ?? 0);
}