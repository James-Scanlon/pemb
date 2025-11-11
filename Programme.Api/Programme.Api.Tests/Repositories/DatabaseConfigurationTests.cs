using System;
using FluentAssertions;
using NUnit.Framework;
using Programme.Api.Repositories;

namespace Programme.Api.Tests.Repositories;

public class DatabaseConfigurationTests
{
    [Test]
    public void Ctor_CalledWithNullConfiguration_ShouldThrowArgumentNullException()
    {
        var func = () => new DatabaseConfiguration(null);
        func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("configuration");
    }
}