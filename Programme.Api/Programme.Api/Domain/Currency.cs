using System;

namespace Programme.Api.Domain;

public sealed class Currency
{
    public string Code { get; set; }

    private bool Equals(Currency other)
    {
        return string.Equals(Code, other.Code, StringComparison.InvariantCultureIgnoreCase);
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Currency)obj);
    }

    public override int GetHashCode()
    {
        return StringComparer.InvariantCultureIgnoreCase.GetHashCode(Code);
    }
}