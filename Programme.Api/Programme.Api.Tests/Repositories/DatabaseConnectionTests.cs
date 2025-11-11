using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Pemberton.KeyVault;
using Programme.Api.Repositories;

namespace Programme.Api.Tests.Repositories;

public class DatabaseConnectionTests
{
    private readonly Mock<IDatabaseConfiguration> _mockConfig;
    private readonly Mock<IKeyVaultService> _mockKeyVaultService;

    public DatabaseConnectionTests()
    {
        _mockConfig = new Mock<IDatabaseConfiguration>();
        _mockKeyVaultService = new Mock<IKeyVaultService>();
    }

    [Test]
    public void Ctor_CalledWithNullDatabaseConfiguration_ShouldThrowArgumentNullException()
    {
        var func = () => new DatabaseConnection(null, _mockKeyVaultService.Object);
        func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("config");
    }

    [Test]
    public void Ctor_CalledWithNullKeyVaultService_ShouldThrowArgumentNullException()
    {
        var func = () => new DatabaseConnection(_mockConfig.Object, null);
        func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("keyVaultService");
    }
}