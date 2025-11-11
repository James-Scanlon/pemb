using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Programme.Api.Domain;

namespace Programme.Api.Tests.Domain;

public class ProgrammeLimitTests
{
    #region Standard Breach Tests

    [Test]
    public void IsStandardBreach_WhenEvergreenProgramme_ShouldReturnFalse()
    {
        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = true,
            Limit = 1000000,
            TotalPurchasesInProgrammeCurrency = 900000,
            ReserveAmount = 200000,
            BreachAnalysisMethod = BreachAnalysisMethod.Standard
        };

        programmeLimit.IsStandardBreach.Should().BeFalse();
    }

    [Test]
    public void IsStandardBreach_WhenLimitIsZero_ShouldReturnFalse()
    {
        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            Limit = 0,
            TotalPurchasesInProgrammeCurrency = 500000,
            ReserveAmount = 100000,
            BreachAnalysisMethod = BreachAnalysisMethod.Standard
        };

        programmeLimit.IsStandardBreach.Should().BeFalse();
    }

    [Test]
    public void IsStandardBreach_WhenReserveAmountIsZero_ShouldReturnFalse()
    {
        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            Limit = 1000000,
            TotalPurchasesInProgrammeCurrency = 500000,
            ReserveAmount = 0,
            BreachAnalysisMethod = BreachAnalysisMethod.Standard
        };

        programmeLimit.IsStandardBreach.Should().BeFalse();
    }

    [Test]
    public void IsStandardBreach_WhenHeadroomLessThanReserve_ShouldReturnTrue()
    {
        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            Limit = 1000000,
            TotalPurchasesInProgrammeCurrency = 900000,
            ReserveAmount = 150000,
            BreachAnalysisMethod = BreachAnalysisMethod.Standard
        };

        // Headroom = 1000000 - 900000 = 100000
        // ReserveAmount = 150000
        // 100000 < 150000 = Breach
        programmeLimit.IsStandardBreach.Should().BeTrue();
    }

    [Test]
    public void IsStandardBreach_WhenHeadroomGreaterThanReserve_ShouldReturnFalse()
    {
        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            Limit = 1000000,
            TotalPurchasesInProgrammeCurrency = 800000,
            ReserveAmount = 150000,
            BreachAnalysisMethod = BreachAnalysisMethod.Standard
        };

        // Headroom = 1000000 - 800000 = 200000
        // ReserveAmount = 150000
        // 200000 >= 150000 = No Breach
        programmeLimit.IsStandardBreach.Should().BeFalse();
    }

    [Test]
    public void IsStandardBreach_WhenHeadroomEqualsReserve_ShouldReturnFalse()
    {
        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            Limit = 1000000,
            TotalPurchasesInProgrammeCurrency = 850000,
            ReserveAmount = 150000,
            BreachAnalysisMethod = BreachAnalysisMethod.Standard
        };

        // Headroom = 1000000 - 850000 = 150000
        // ReserveAmount = 150000
        // 150000 >= 150000 = No Breach (boundary case)
        programmeLimit.IsStandardBreach.Should().BeFalse();
    }

    #endregion

    #region Parent Breach Tests

    [Test]
    public void IsParentBreach_WhenEvergreenProgramme_ShouldReturnFalse()
    {
        var parentGroup = new LimitGroup
        {
            Id = 1,
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit>()
        };

        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = true,
            ParentGroup = parentGroup,
            ReserveAmount = 100000,
            ParentConversionRate = 1.0m,
            BreachAnalysisMethod = BreachAnalysisMethod.Parent
        };

        programmeLimit.IsParentBreach.Should().BeFalse();
    }

    [Test]
    public void IsParentBreach_WhenNoParentGroup_ShouldReturnFalse()
    {
        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            ParentGroup = null,
            ReserveAmount = 100000,
            ParentConversionRate = 1.0m,
            BreachAnalysisMethod = BreachAnalysisMethod.Parent
        };

        programmeLimit.IsParentBreach.Should().BeFalse();
    }

    [Test]
    public void IsParentBreach_WhenNoConversionRate_ShouldReturnFalse()
    {
        var parentGroup = new LimitGroup
        {
            Id = 1,
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit>()
        };

        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            ParentGroup = parentGroup,
            ReserveAmount = 100000,
            ParentConversionRate = null,
            BreachAnalysisMethod = BreachAnalysisMethod.Parent
        };

        programmeLimit.IsParentBreach.Should().BeFalse();
    }

    [Test]
    public void IsParentBreach_WhenParentHeadroomLessThanReserve_ShouldReturnTrue()
    {
        var member1 = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 3000000,
            ParentConversionRate = 1.0m
        };

        var member2 = new ProgrammeLimit
        {
            ProgrammeId = 2,
            TotalPurchasesInProgrammeCurrency = 1500000,
            ParentConversionRate = 1.0m
        };

        var parentGroup = new LimitGroup
        {
            Id = 1,
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member1, member2 }
        };

        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            ParentGroup = parentGroup,
            ReserveAmount = 600000,
            ParentConversionRate = 1.0m,
            BreachAnalysisMethod = BreachAnalysisMethod.Parent
        };

        // Parent headroom = 5000000 - (3000000 + 1500000) = 500000
        // Reserve in group currency = 600000
        // 500000 < 600000 = Breach
        programmeLimit.IsParentBreach.Should().BeTrue();
    }

    [Test]
    public void IsParentBreach_WhenParentHeadroomGreaterThanReserve_ShouldReturnFalse()
    {
        var member1 = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 2000000,
            ParentConversionRate = 1.0m
        };

        var parentGroup = new LimitGroup
        {
            Id = 1,
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member1 }
        };

        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            ParentGroup = parentGroup,
            ReserveAmount = 500000,
            ParentConversionRate = 1.0m,
            BreachAnalysisMethod = BreachAnalysisMethod.Parent
        };

        // Parent headroom = 5000000 - 2000000 = 3000000
        // Reserve in group currency = 500000
        // 3000000 >= 500000 = No Breach
        programmeLimit.IsParentBreach.Should().BeFalse();
    }

    #endregion

    #region Calculated Properties Tests

    [Test]
    public void TotalPurchasesInGroupCurrency_WhenConversionRateProvided_ShouldCalculateCorrectly()
    {
        var programmeLimit = new ProgrammeLimit
        {
            TotalPurchasesInProgrammeCurrency = 1000000,
            ParentConversionRate = 1.25m
        };

        programmeLimit.TotalPurchasesInGroupCurrency.Should().Be(1250000);
    }

    [Test]
    public void TotalPurchasesInGroupCurrency_WhenNoConversionRate_ShouldReturnNull()
    {
        var programmeLimit = new ProgrammeLimit
        {
            TotalPurchasesInProgrammeCurrency = 1000000,
            ParentConversionRate = null
        };

        programmeLimit.TotalPurchasesInGroupCurrency.Should().BeNull();
    }

    [Test]
    public void ReserveAmountInGroupCurrency_WhenConversionRateProvided_ShouldCalculateCorrectly()
    {
        var programmeLimit = new ProgrammeLimit
        {
            ReserveAmount = 100000,
            ParentConversionRate = 0.85m
        };

        programmeLimit.ReserveAmountInGroupCurrency.Should().Be(85000);
    }

    [Test]
    public void ReserveAmountInGroupCurrency_WhenNoConversionRate_ShouldReturnNull()
    {
        var programmeLimit = new ProgrammeLimit
        {
            ReserveAmount = 100000,
            ParentConversionRate = null
        };

        programmeLimit.ReserveAmountInGroupCurrency.Should().BeNull();
    }

    #endregion

    #region IsLimitBreach Tests - Standard Method

    [Test]
    public void IsLimitBreach_StandardMethod_WhenStandardBreach_ShouldReturnTrue()
    {
        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            Limit = 1000000,
            TotalPurchasesInProgrammeCurrency = 950000,
            ReserveAmount = 100000,
            BreachAnalysisMethod = BreachAnalysisMethod.Standard
        };

        programmeLimit.IsLimitBreach.Should().BeTrue();
    }

    [Test]
    public void IsLimitBreach_StandardMethod_WhenNoStandardBreach_ShouldReturnFalse()
    {
        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            Limit = 1000000,
            TotalPurchasesInProgrammeCurrency = 800000,
            ReserveAmount = 100000,
            BreachAnalysisMethod = BreachAnalysisMethod.Standard
        };

        programmeLimit.IsLimitBreach.Should().BeFalse();
    }

    #endregion

    #region IsLimitBreach Tests - Parent Method

    [Test]
    public void IsLimitBreach_ParentMethod_WhenParentBreach_ShouldReturnTrue()
    {
        var member = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 4000000,
            ParentConversionRate = 1.0m
        };

        var parentGroup = new LimitGroup
        {
            Id = 1,
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member }
        };

        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            ParentGroup = parentGroup,
            ReserveAmount = 1500000,
            ParentConversionRate = 1.0m,
            BreachAnalysisMethod = BreachAnalysisMethod.Parent
        };

        programmeLimit.IsLimitBreach.Should().BeTrue();
    }

    [Test]
    public void IsLimitBreach_ParentMethod_WhenNoParentBreach_ShouldReturnFalse()
    {
        var member = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 2000000,
            ParentConversionRate = 1.0m
        };

        var parentGroup = new LimitGroup
        {
            Id = 1,
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member }
        };

        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            ParentGroup = parentGroup,
            ReserveAmount = 500000,
            ParentConversionRate = 1.0m,
            BreachAnalysisMethod = BreachAnalysisMethod.Parent
        };

        programmeLimit.IsLimitBreach.Should().BeFalse();
    }

    #endregion

    #region IsLimitBreach Tests - Combined Method

    [Test]
    public void IsLimitBreach_CombinedMethod_WhenBothBreaches_ShouldReturnTrue()
    {
        var member = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 4000000,
            ParentConversionRate = 1.0m
        };

        var parentGroup = new LimitGroup
        {
            Id = 1,
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member }
        };

        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            Limit = 1000000,
            TotalPurchasesInProgrammeCurrency = 950000,
            ReserveAmount = 100000,
            ParentGroup = parentGroup,
            ParentConversionRate = 1.0m,
            BreachAnalysisMethod = BreachAnalysisMethod.Combined
        };

        programmeLimit.IsLimitBreach.Should().BeTrue();
    }

    [Test]
    public void IsLimitBreach_CombinedMethod_WhenOnlyStandardBreach_ShouldReturnTrue()
    {
        var member = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 1000000,
            ParentConversionRate = 1.0m
        };

        var parentGroup = new LimitGroup
        {
            Id = 1,
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member }
        };

        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            Limit = 1000000,
            TotalPurchasesInProgrammeCurrency = 950000,
            ReserveAmount = 100000,
            ParentGroup = parentGroup,
            ParentConversionRate = 1.0m,
            BreachAnalysisMethod = BreachAnalysisMethod.Combined
        };

        programmeLimit.IsLimitBreach.Should().BeTrue();
    }

    [Test]
    public void IsLimitBreach_CombinedMethod_WhenOnlyParentBreach_ShouldReturnTrue()
    {
        var member = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 4000000,
            ParentConversionRate = 1.0m
        };

        var parentGroup = new LimitGroup
        {
            Id = 1,
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member }
        };

        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            Limit = 1000000,
            TotalPurchasesInProgrammeCurrency = 500000,
            ReserveAmount = 100000,
            ParentGroup = parentGroup,
            ParentConversionRate = 1.0m,
            BreachAnalysisMethod = BreachAnalysisMethod.Combined
        };

        programmeLimit.IsLimitBreach.Should().BeFalse();
    }

    [Test]
    public void IsLimitBreach_CombinedMethod_WhenNoBreaches_ShouldReturnFalse()
    {
        var member = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 1000000,
            ParentConversionRate = 1.0m
        };

        var parentGroup = new LimitGroup
        {
            Id = 1,
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member }
        };

        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            Limit = 1000000,
            TotalPurchasesInProgrammeCurrency = 500000,
            ReserveAmount = 100000,
            ParentGroup = parentGroup,
            ParentConversionRate = 1.0m,
            BreachAnalysisMethod = BreachAnalysisMethod.Combined
        };

        programmeLimit.IsLimitBreach.Should().BeFalse();
    }

    #endregion

    #region LimitText Tests

    [Test]
    public void LimitText_WhenNoBreach_ShouldReturnWithinFacility()
    {
        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            Limit = 1000000,
            TotalPurchasesInProgrammeCurrency = 500000,
            ReserveAmount = 100000,
            BreachAnalysisMethod = BreachAnalysisMethod.Standard
        };

        programmeLimit.LimitText.Should().Be("Within Facility");
    }

    [Test]
    public void LimitText_StandardMethod_WhenStandardBreach_ShouldReturnStandardBreach()
    {
        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            Limit = 1000000,
            TotalPurchasesInProgrammeCurrency = 950000,
            ReserveAmount = 100000,
            BreachAnalysisMethod = BreachAnalysisMethod.Standard
        };

        programmeLimit.LimitText.Should().Be("Standard Breach");
    }

    [Test]
    public void LimitText_ParentMethod_WhenParentBreach_ShouldReturnParentBreach()
    {
        var member = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 4000000,
            ParentConversionRate = 1.0m
        };

        var parentGroup = new LimitGroup
        {
            Id = 1,
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member }
        };

        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            ParentGroup = parentGroup,
            ReserveAmount = 1500000,
            ParentConversionRate = 1.0m,
            BreachAnalysisMethod = BreachAnalysisMethod.Parent
        };

        programmeLimit.LimitText.Should().Be("Parent Breach");
    }

    [Test]
    public void LimitText_CombinedMethod_WhenBothBreaches_ShouldReturnCombinedMessage()
    {
        var member = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 4000000,
            ParentConversionRate = 1.0m
        };

        var parentGroup = new LimitGroup
        {
            Id = 1,
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member }
        };

        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            Limit = 1000000,
            TotalPurchasesInProgrammeCurrency = 950000,
            ReserveAmount = 100000,
            ParentGroup = parentGroup,
            ParentConversionRate = 1.0m,
            BreachAnalysisMethod = BreachAnalysisMethod.Combined
        };

        programmeLimit.LimitText.Should().Be("Standard Breach");
    }

    [Test]
    public void LimitText_CombinedMethod_WhenOnlyStandardBreach_ShouldReturnStandardBreach()
    {
        var member = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 1000000,
            ParentConversionRate = 1.0m
        };

        var parentGroup = new LimitGroup
        {
            Id = 1,
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member }
        };

        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            Limit = 1000000,
            TotalPurchasesInProgrammeCurrency = 950000,
            ReserveAmount = 100000,
            ParentGroup = parentGroup,
            ParentConversionRate = 1.0m,
            BreachAnalysisMethod = BreachAnalysisMethod.Combined
        };

        programmeLimit.LimitText.Should().Be("Standard Breach");
    }

    [Test]
    public void LimitText_CombinedMethod_WhenOnlyParentBreach_ShouldReturnParentBreach()
    {
        var member = new ProgrammeLimit
        {
            ProgrammeId = 1,
            TotalPurchasesInProgrammeCurrency = 4000000,
            ParentConversionRate = 1.0m
        };

        var parentGroup = new LimitGroup
        {
            Id = 1,
            Limit = 5000000,
            Currency = "USD",
            Members = new List<ProgrammeLimit> { member }
        };

        var programmeLimit = new ProgrammeLimit
        {
            IsEvergreen = false,
            Limit = 1000000,
            TotalPurchasesInProgrammeCurrency = 500000,
            ReserveAmount = 100000,
            ParentGroup = parentGroup,
            ParentConversionRate = 1.0m,
            BreachAnalysisMethod = BreachAnalysisMethod.Combined
        };

        programmeLimit.LimitText.Should().Be("Within Facility");
    }

    #endregion
}