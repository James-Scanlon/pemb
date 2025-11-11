using FluentAssertions;
using NUnit.Framework;
using Programme.Api.Domain;

namespace Programme.Api.Tests.Domain;

public class CurrencyTests
{
    [Test]
    public void Equals_WhenSameCurrencyCode_ShouldReturnTrue()
    {
        var currency1 = new Currency { Code = "USD" };
        var currency2 = new Currency { Code = "USD" };

        currency1.Equals(currency2).Should().BeTrue();
    }

    [Test]
    public void Equals_WhenSameCurrencyCodeDifferentCase_ShouldReturnTrue()
    {
        var currency1 = new Currency { Code = "USD" };
        var currency2 = new Currency { Code = "usd" };

        currency1.Equals(currency2).Should().BeTrue();
    }

    [Test]
    public void Equals_WhenSameCurrencyCodeMixedCase_ShouldReturnTrue()
    {
        var currency1 = new Currency { Code = "GBP" };
        var currency2 = new Currency { Code = "gbp" };

        currency1.Equals(currency2).Should().BeTrue();
    }

    [Test]
    public void Equals_WhenDifferentCurrencyCode_ShouldReturnFalse()
    {
        var currency1 = new Currency { Code = "USD" };
        var currency2 = new Currency { Code = "EUR" };

        currency1.Equals(currency2).Should().BeFalse();
    }

    [Test]
    public void Equals_WhenOtherIsNull_ShouldReturnFalse()
    {
        var currency = new Currency { Code = "USD" };

        currency.Equals(null).Should().BeFalse();
    }

    [Test]
    public void Equals_WhenSameReference_ShouldReturnTrue()
    {
        var currency = new Currency { Code = "USD" };

        currency.Equals(currency).Should().BeTrue();
    }

    [Test]
    public void Equals_WhenDifferentType_ShouldReturnFalse()
    {
        var currency = new Currency { Code = "USD" };
        var notACurrency = "USD";

        currency.Equals(notACurrency).Should().BeFalse();
    }

    [Test]
    public void GetHashCode_WhenSameCurrencyCode_ShouldReturnSameHashCode()
    {
        var currency1 = new Currency { Code = "USD" };
        var currency2 = new Currency { Code = "USD" };

        currency1.GetHashCode().Should().Be(currency2.GetHashCode());
    }

    [Test]
    public void GetHashCode_WhenSameCurrencyCodeDifferentCase_ShouldReturnSameHashCode()
    {
        var currency1 = new Currency { Code = "USD" };
        var currency2 = new Currency { Code = "usd" };

        currency1.GetHashCode().Should().Be(currency2.GetHashCode());
    }

    [Test]
    public void GetHashCode_WhenDifferentCurrencyCode_ShouldReturnDifferentHashCode()
    {
        var currency1 = new Currency { Code = "USD" };
        var currency2 = new Currency { Code = "EUR" };

        currency1.GetHashCode().Should().NotBe(currency2.GetHashCode());
    }
}