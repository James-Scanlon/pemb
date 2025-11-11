using System;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Pemberton.Rates.Api.Client;
using Programme.Api.Repositories;
using Wcf.Accounts.ApiClient;

namespace Programme.Api.Tests.Repositories;

public class ProgrammeRepositoryTests
{
    private readonly Mock<IDatabaseConnection> _mockDatabaseConnection = new();
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<IMemoryCache> _mockMemoryCache = new();
    private readonly Mock<ILogger<ProgrammeRepository>> _mockLogger = new();
    private readonly Mock<IAccountsApiClient> _accountsApiClient = new();
    private readonly Mock<IRatesApiClient> _ratesApiClient = new();
    private readonly Mock<INextIdRepository> _nextIdRepository = new();

    [Test]
    public void Ctor_CalledWithNullDatabaseConnection_ShouldThrowArgumentNullException()
    {
        var func = () => new ProgrammeRepository(null, _mockMapper.Object, _mockMemoryCache.Object, _mockLogger.Object,
            _accountsApiClient.Object, _ratesApiClient.Object, _nextIdRepository.Object);

        func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("databaseConnection");
    }

    [Test]
    public void Ctor_CalledWithNullMapper_ShouldThrowArgumentNullException()
    {
        var func = () => new ProgrammeRepository(_mockDatabaseConnection.Object, null, _mockMemoryCache.Object,
            _mockLogger.Object, _accountsApiClient.Object, _ratesApiClient.Object, _nextIdRepository.Object);

        func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("mapper");
    }

    [Test]
    public void Ctor_CalledWithNullMemoryCache_ShouldThrowArgumentNullException()
    {
        var func = () => new ProgrammeRepository(_mockDatabaseConnection.Object, _mockMapper.Object, null,
            _mockLogger.Object, _accountsApiClient.Object, _ratesApiClient.Object, _nextIdRepository.Object);

        func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("memoryCache");
    }

    [Test]
    public void Ctor_CalledWithNullLogger_ShouldThrowArgumentNullException()
    {
        var func = () => new ProgrammeRepository(_mockDatabaseConnection.Object, _mockMapper.Object,
            _mockMemoryCache.Object, null, _accountsApiClient.Object, _ratesApiClient.Object, _nextIdRepository.Object);

        func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("logger");
    }

    [Test]
    public void Ctor_CalledWithNullAccountRepository_ShouldThrowArgumentNullException()
    {
        var func = () => new ProgrammeRepository(_mockDatabaseConnection.Object, _mockMapper.Object,
            _mockMemoryCache.Object, _mockLogger.Object, null, _ratesApiClient.Object, _nextIdRepository.Object);

        func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("accountsApiClient");
    }
}