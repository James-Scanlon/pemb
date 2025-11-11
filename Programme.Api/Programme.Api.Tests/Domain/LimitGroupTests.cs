using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Programme.Api.Domain;

namespace Programme.Api.Tests.Domain;

public class LimitGroupTests
{
    [Test]
    public void GroupTotalPurchasePrice_WhenNoMembers_ShouldReturnZero()
    {
        var limitGroup = new LimitGroup
        {
            Id = 1,
            Name = "Test Group",
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit>()
        };

        limitGroup.GroupTotalPurchasePrice.Should().Be(0);
    }

    [Test]
    public void GroupTotalPurchasePrice_WhenSingleMember_ShouldReturnMemberTotal()
    {
        var member = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 1000000,
            ParentConversionRate = 1.0m
        };

        var limitGroup = new LimitGroup
        {
            Id = 1,
            Name = "Test Group",
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member }
        };

        limitGroup.GroupTotalPurchasePrice.Should().Be(1000000);
    }

    [Test]
    public void GroupTotalPurchasePrice_WhenMultipleMembers_ShouldReturnSum()
    {
        var member1 = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 1000000,
            ParentConversionRate = 1.0m
        };

        var member2 = new ProgrammeLimit
        {
            ProgrammeId = 2,
            TotalPurchasesInProgrammeCurrency = 2500000,
            ParentConversionRate = 1.0m
        };

        var member3 = new ProgrammeLimit
        {
            ProgrammeId = 3,
            TotalPurchasesInProgrammeCurrency = 1500000,
            ParentConversionRate = 1.0m
        };

        var limitGroup = new LimitGroup
        {
            Id = 1,
            Name = "Test Group",
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member1, member2, member3 }
        };

        limitGroup.GroupTotalPurchasePrice.Should().Be(5000000);
    }

    [Test]
    public void GroupTotalPurchasePrice_WhenMembersWithDifferentConversionRates_ShouldCalculateCorrectly()
    {
        var member1 = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 1000000, // GBP
            ParentConversionRate = 1.25m // GBP to USD
        };

        var member2 = new ProgrammeLimit
        {
            ProgrammeId = 2,
            TotalPurchasesInProgrammeCurrency = 2000000, // USD
            ParentConversionRate = 1.0m
        };

        var member3 = new ProgrammeLimit
        {
            ProgrammeId = 3,
            TotalPurchasesInProgrammeCurrency = 1000000, // EUR
            ParentConversionRate = 1.1m // EUR to USD
        };

        var limitGroup = new LimitGroup
        {
            Id = 1,
            Name = "Multi-Currency Group",
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member1, member2, member3 }
        };

        // (1000000 * 1.25) + (2000000 * 1.0) + (1000000 * 1.1) = 1250000 + 2000000 + 1100000 = 4350000
        limitGroup.GroupTotalPurchasePrice.Should().Be(4350000);
    }

    [Test]
    public void GroupTotalPurchasePrice_WhenMemberHasNullConversionRate_ShouldTreatAsZero()
    {
        var member1 = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 1000000,
            ParentConversionRate = 1.0m
        };

        var member2 = new ProgrammeLimit
        {
            ProgrammeId = 2,
            TotalPurchasesInProgrammeCurrency = 2000000,
            ParentConversionRate = null // No conversion rate
        };

        var limitGroup = new LimitGroup
        {
            Id = 1,
            Name = "Test Group",
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member1, member2 }
        };

        // member1: 1000000 * 1.0 = 1000000
        // member2: null treated as 0
        limitGroup.GroupTotalPurchasePrice.Should().Be(1000000);
    }

    [Test]
    public void GroupTotalPurchasePrice_WhenAllMembersHaveNullConversionRate_ShouldReturnZero()
    {
        var member1 = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 1000000,
            ParentConversionRate = null
        };

        var member2 = new ProgrammeLimit
        {
            ProgrammeId = 2,
            TotalPurchasesInProgrammeCurrency = 2000000,
            ParentConversionRate = null
        };

        var limitGroup = new LimitGroup
        {
            Id = 1,
            Name = "Test Group",
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member1, member2 }
        };

        limitGroup.GroupTotalPurchasePrice.Should().Be(0);
    }
}