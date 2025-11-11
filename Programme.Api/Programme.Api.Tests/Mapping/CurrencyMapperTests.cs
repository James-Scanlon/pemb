using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Programme.Api.Domain;
using Programme.Api.Mapper;
using Programme.Api.Repositories;
namespace Wcf.Allocations.Api.Tests.Mapping;

public class CurrencyMapperTests
{
    private readonly Mock<ICurrencyRepository> _mockCurrencyRepository;

    public CurrencyMapperTests()
    {
        _mockCurrencyRepository = new Mock<ICurrencyRepository>();
    }

    [Test]
    public void Ctor_CalledWithNullCurrencyRepository_ShouldThrowArgumentNullException()
    {
        var func = () => new CurrencyMapper(null);
        func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("currencyRepository");
    }

    [Test]
    public void Convert_CalledWithKnownCurrencyCode_ShouldMapToCurrency()
    {
        var expected = It.IsAny<Currency>();

        _mockCurrencyRepository.Setup(x => x.GetByCodeAsync(It.Is<string>(y => y == "XYZ")))
            .ReturnsAsync(expected);

        var target = DefaultSubjectUnderTest();
        var actual = target.Convert("XYZ", null);

        actual.Should().Be(expected);
    }

    [Test]
    public void Convert_CalledWithUnknownCurrencyCode_ShouldReturnNull()
    {
        _mockCurrencyRepository.Setup(x => x.GetByCodeAsync(It.Is<string>(y => y == "XYZ")))
            .ReturnsAsync(default(Currency));

        var target = DefaultSubjectUnderTest();
        var actual = target.Convert("XYZ", null);

        actual.Should().BeNull();
    }

    private CurrencyMapper DefaultSubjectUnderTest()
    {
        return new CurrencyMapper(_mockCurrencyRepository.Object);
    }
}