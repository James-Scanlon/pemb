using System;
using FluentAssertions;
using NUnit.Framework;
using Programme.Api.Domain;

namespace Programme.Api.Tests.Domain;

public class EvergreenPositionDataTests
{
    [Test]
    public void CanPayDown_WhenAllConditionsMet_ShouldReturnTrue()
    {
        var position = new EvergreenPositionData
        {
            AmountOutstanding = 100000,
            HasAllocation = true,
            HasUnallocatedEvergreenBatch = false
        };

        position.CanPayDown.Should().BeTrue();
    }

    [Test]
    public void CanPayDown_WhenAmountOutstandingIsZero_ShouldReturnFalse()
    {
        var position = new EvergreenPositionData
        {
            AmountOutstanding = 0,
            HasAllocation = true,
            HasUnallocatedEvergreenBatch = false
        };

        position.CanPayDown.Should().BeFalse();
    }

    [Test]
    public void CanPayDown_WhenAmountOutstandingIsNegative_ShouldReturnFalse()
    {
        var position = new EvergreenPositionData
        {
            AmountOutstanding = -100000,
            HasAllocation = true,
            HasUnallocatedEvergreenBatch = false
        };

        position.CanPayDown.Should().BeFalse();
    }

    [Test]
    public void CanPayDown_WhenNoAllocation_ShouldReturnFalse()
    {
        var position = new EvergreenPositionData
        {
            AmountOutstanding = 100000,
            HasAllocation = false,
            HasUnallocatedEvergreenBatch = false
        };

        position.CanPayDown.Should().BeFalse();
    }

    [Test]
    public void CanPayDown_WhenHasUnallocatedEvergreenBatch_ShouldReturnFalse()
    {
        var position = new EvergreenPositionData
        {
            AmountOutstanding = 100000,
            HasAllocation = true,
            HasUnallocatedEvergreenBatch = true
        };

        position.CanPayDown.Should().BeFalse();
    }

    [Test]
    public void CanPayDown_WhenNoAllocationAndHasUnallocatedBatch_ShouldReturnFalse()
    {
        var position = new EvergreenPositionData
        {
            AmountOutstanding = 100000,
            HasAllocation = false,
            HasUnallocatedEvergreenBatch = true
        };

        position.CanPayDown.Should().BeFalse();
    }

    [Test]
    public void CanPayDown_WhenAmountIsSmall_ShouldReturnTrue()
    {
        var position = new EvergreenPositionData
        {
            AmountOutstanding = 0.01m,
            HasAllocation = true,
            HasUnallocatedEvergreenBatch = false
        };

        position.CanPayDown.Should().BeTrue();
    }

    [Test]
    public void Properties_ShouldSetAndGetCorrectly()
    {
        var position = new EvergreenPositionData
        {
            ProgrammeID = 123,
            Currency = "USD",
            ProgrammeName = "Test Programme",
            AmountOutstanding = 500000,
            HasAllocation = true,
            CurrentPayDownValue = 100000,
            CurrentPayDownDate = new DateTime(2024, 1, 15),
            CurrentPayDownPaymentId = "PAY123",
            CurrentPayDownComments = "Test payment",
            BatchIdentifier = "BATCH001",
            BatchAllocationCandidateIdentifier = "ALLOC001",
            HasUnallocatedEvergreenBatch = false
        };

        position.ProgrammeID.Should().Be(123);
        position.Currency.Should().Be("USD");
        position.ProgrammeName.Should().Be("Test Programme");
        position.AmountOutstanding.Should().Be(500000);
        position.HasAllocation.Should().BeTrue();
        position.CurrentPayDownValue.Should().Be(100000);
        position.CurrentPayDownDate.Should().Be(new DateTime(2024, 1, 15));
        position.CurrentPayDownPaymentId.Should().Be("PAY123");
        position.CurrentPayDownComments.Should().Be("Test payment");
        position.BatchIdentifier.Should().Be("BATCH001");
        position.BatchAllocationCandidateIdentifier.Should().Be("ALLOC001");
        position.HasUnallocatedEvergreenBatch.Should().BeFalse();
    }
}