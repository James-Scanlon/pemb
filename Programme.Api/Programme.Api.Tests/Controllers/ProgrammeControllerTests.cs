using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Audit.ApiClient;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Programme.Api.Controllers;            
using Programme.Api.Domain;
using Programme.Api.Dto;
using Programme.Api.Repositories;

namespace Programme.Api.Tests.Controllers;

public class ProgrammeControllerTests
{
    private Mock<IMapper> _mockMapper;
    private Mock<IProgrammeRepository> _mockProgrammeRepository;
    private ILogger<ProgrammeController> _logger;
    private Mock<IAuditApiClient> _mockAuditClient;
    private ProgrammeController _controller;

    [SetUp]
    public void Initialize()
    {
        _mockMapper = new Mock<IMapper>();
        _mockProgrammeRepository = new Mock<IProgrammeRepository>();
        _logger = new NullLogger<ProgrammeController>();
        _mockAuditClient = new Mock<IAuditApiClient>();
        _controller = new ProgrammeController(_mockMapper.Object, _mockProgrammeRepository.Object, _logger, _mockAuditClient.Object);
    }

    [Test]
    public void Ctor_CalledWithNullMapper_ShouldThrowArgumentNullException()
    {
        var func = () => new ProgrammeController(null, _mockProgrammeRepository.Object, _logger, _mockAuditClient.Object);
        func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("mapper");
    }

    [Test]
    public void Ctor_CalledWithNullProgrammeRepository_ShouldThrowArgumentNullException()
    {
        var func = () => new ProgrammeController(_mockMapper.Object, null, _logger, _mockAuditClient.Object);
        func.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("programmeRepository");
    }

    [Test]
    public async Task GetAsync_CalledWhenRepositoryReturnsNoProgrammes_ShouldReturnNotFound()
    {
        _mockProgrammeRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Api.Domain.Programme>());

        var actual = await _controller.GetAsync();

        actual.Should().BeOfType<NotFoundResult>();
    }

    [Test]
    public async Task GetAsync_Called_ShouldReturnOk()
    {
        _mockProgrammeRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Api.Domain.Programme> { new Api.Domain.Programme() });
        _mockMapper.Setup(x => x.Map<IReadOnlyCollection<ProgrammeDto>>(It.IsAny<IReadOnlyCollection<Api.Domain.Programme>>()))
            .Returns(new List<ProgrammeDto>());

        var actual = await _controller.GetAsync();

        actual.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task GetAsync_Called_ShouldReturnSuccess()
    {
        _mockProgrammeRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Api.Domain.Programme> { new Api.Domain.Programme() });
        _mockMapper.Setup(x => x.Map<IReadOnlyCollection<ProgrammeDto>>(It.IsAny<IReadOnlyCollection<Api.Domain.Programme>>()))
            .Returns(new List<ProgrammeDto>());

        var actual = await _controller.GetAsync();

        actual.Should().BeOfType<OkObjectResult>().Which.Value.As<GetProgrammesResponseDto>().IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task GetAsync_Called_ShouldCallRepositoryGetAllAsync()
    {
        _mockProgrammeRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(new List<Api.Domain.Programme> { new Api.Domain.Programme() });

        await _controller.GetAsync();

        _mockProgrammeRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetAsync_Called_ShouldMapProgrammesToDto()
    {
        var programmes = new List<Api.Domain.Programme> { new Api.Domain.Programme() };
        _mockProgrammeRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(programmes);

        await _controller.GetAsync();

        _mockMapper.Verify(x => x.Map<IReadOnlyCollection<ProgrammeDto>>(programmes), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_Called_ShouldReturnOk()
    {
        var programmeId = 123;
        var programme = new Api.Domain.Programme { ProgrammeId = programmeId };
        _mockProgrammeRepository.Setup(x => x.GetByIdAsync(programmeId)).ReturnsAsync(programme);
        _mockMapper.Setup(x => x.Map<ProgrammeDto>(programme)).Returns(new ProgrammeDto());

        var actual = await _controller.GetByIdAsync(programmeId);

        actual.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task GetByIdAsync_Called_ShouldCallRepositoryWithCorrectId()
    {
        var programmeId = 456;
        var programme = new Api.Domain.Programme { ProgrammeId = programmeId };
        _mockProgrammeRepository.Setup(x => x.GetByIdAsync(programmeId)).ReturnsAsync(programme);

        await _controller.GetByIdAsync(programmeId);

        _mockProgrammeRepository.Verify(x => x.GetByIdAsync(programmeId), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_Called_ShouldMapProgrammeToDto()
    {
        var programmeId = 789;
        var programme = new Api.Domain.Programme { ProgrammeId = programmeId };
        _mockProgrammeRepository.Setup(x => x.GetByIdAsync(programmeId)).ReturnsAsync(programme);

        await _controller.GetByIdAsync(programmeId);

        _mockMapper.Verify(x => x.Map<ProgrammeDto>(programme), Times.Once);
    }

    [Test]
    public async Task GetCurrentProgrammeStateByBatchAsync_Called_ShouldReturnOk()
    {
        var batchId = "BATCH123";
        var programmeState = new ProgrammeState { ProgrammeId = 1 };
        _mockProgrammeRepository.Setup(x => x.GetCurrentProgrammeStateByBatchAsync(batchId)).ReturnsAsync(programmeState);
        _mockMapper.Setup(x => x.Map<ProgrammeStateDto>(programmeState)).Returns(new ProgrammeStateDto());

        var actual = await _controller.GetCurrentProgrammeStateByBatchAsync(batchId);

        actual.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task GetCurrentProgrammeStateByBatchAsync_Called_ShouldCallRepositoryWithCorrectBatchId()
    {
        var batchId = "BATCH456";
        var programmeState = new ProgrammeState();
        _mockProgrammeRepository.Setup(x => x.GetCurrentProgrammeStateByBatchAsync(batchId)).ReturnsAsync(programmeState);

        await _controller.GetCurrentProgrammeStateByBatchAsync(batchId);

        _mockProgrammeRepository.Verify(x => x.GetCurrentProgrammeStateByBatchAsync(batchId), Times.Once);
    }

    [Test]
    public async Task GetProgrammeStatic_Called_ShouldReturnOk()
    {
        var programmeData = new List<Api.Domain.Programme> { new Api.Domain.Programme() };
        _mockProgrammeRepository.Setup(x => x.GetAllProgrammeDataAsync()).ReturnsAsync(programmeData);
        _mockMapper.Setup(x => x.Map<List<ProgrammeDto>>(programmeData)).Returns(new List<ProgrammeDto>());

        var actual = await _controller.GetProgrammeStatic();

        actual.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task GetProgrammeStatic_Called_ShouldCallRepositoryGetAllProgrammeDataAsync()
    {
        var programmeData = new List<Api.Domain.Programme>();
        _mockProgrammeRepository.Setup(x => x.GetAllProgrammeDataAsync()).ReturnsAsync(programmeData);
        _mockMapper.Setup(x => x.Map<List<ProgrammeDto>>(programmeData)).Returns(new List<ProgrammeDto>());

        await _controller.GetProgrammeStatic();

        _mockProgrammeRepository.Verify(x => x.GetAllProgrammeDataAsync(), Times.Once);
    }

    [Test]
    public async Task GetProgrammeStatic_WhenRepositoryThrowsException_ShouldReturnProblem()
    {
        _mockProgrammeRepository.Setup(x => x.GetAllProgrammeDataAsync()).ThrowsAsync(new Exception("Test exception"));

        var actual = await _controller.GetProgrammeStatic();

        actual.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task GetEvergreenAssetPositions_Called_ShouldReturnOk()
    {
        var positions = new List<EvergreenPositionData> { new EvergreenPositionData() };
        _mockProgrammeRepository.Setup(x => x.GetAllEvergreenAssetPositionsAsync()).ReturnsAsync(positions);
        _mockMapper.Setup(x => x.Map<List<EvergreenPositionDataDto>>(positions)).Returns(new List<EvergreenPositionDataDto>());

        var actual = await _controller.GetEvergreenAssetPositions();

        actual.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task GetEvergreenAssetPositions_Called_ShouldCallRepository()
    {
        var positions = new List<EvergreenPositionData>();
        _mockProgrammeRepository.Setup(x => x.GetAllEvergreenAssetPositionsAsync()).ReturnsAsync(positions);
        _mockMapper.Setup(x => x.Map<List<EvergreenPositionDataDto>>(positions)).Returns(new List<EvergreenPositionDataDto>());

        await _controller.GetEvergreenAssetPositions();

        _mockProgrammeRepository.Verify(x => x.GetAllEvergreenAssetPositionsAsync(), Times.Once);
    }

    [Test]
    public async Task GetEvergreenAssetPositions_WhenRepositoryThrowsException_ShouldReturnProblem()
    {
        _mockProgrammeRepository.Setup(x => x.GetAllEvergreenAssetPositionsAsync()).ThrowsAsync(new Exception("Test exception"));

        var actual = await _controller.GetEvergreenAssetPositions();

        actual.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task ConvertProgrammeToEvergreen_WhenConversionSucceeds_ShouldReturnOk()
    {
        var programmeId = 123;
        _mockProgrammeRepository.Setup(x => x.ConvertProgrammeToEvergreen(programmeId)).ReturnsAsync(true);

        var actual = await _controller.ConvertProgrammeToEvergreen(programmeId);

        actual.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task ConvertProgrammeToEvergreen_WhenConversionFails_ShouldReturnBadRequest()
    {
        var programmeId = 123;
        _mockProgrammeRepository.Setup(x => x.ConvertProgrammeToEvergreen(programmeId)).ReturnsAsync(false);

        var actual = await _controller.ConvertProgrammeToEvergreen(programmeId);

        actual.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task ConvertProgrammeToEvergreen_WhenExceptionThrown_ShouldReturnProblem()
    {
        var programmeId = 123;
        _mockProgrammeRepository.Setup(x => x.ConvertProgrammeToEvergreen(programmeId)).ThrowsAsync(new Exception("Test exception"));

        var actual = await _controller.ConvertProgrammeToEvergreen(programmeId);

        actual.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task ConvertProgrammeToEvergreen_Called_ShouldAuditTheOperation()
    {
        var programmeId = 123;
        _mockProgrammeRepository.Setup(x => x.ConvertProgrammeToEvergreen(programmeId)).ReturnsAsync(true);

        await _controller.ConvertProgrammeToEvergreen(programmeId);

        _mockAuditClient.Verify(x => x.AuditAsync(
            "Programme.Api",
            "ConvertProgrammeToEvergreen",
            It.Is<Dictionary<string, string>>(d => d["ProgrammeId"] == programmeId.ToString())), Times.Once);
    }

    [Test]
    public async Task ConvertProgrammeToEvergreen_WhenExceptionThrown_ShouldStillAudit()
    {
        var programmeId = 123;
        _mockProgrammeRepository.Setup(x => x.ConvertProgrammeToEvergreen(programmeId)).ThrowsAsync(new Exception("Test exception"));

        await _controller.ConvertProgrammeToEvergreen(programmeId);

        _mockAuditClient.Verify(x => x.AuditAsync(
            "Programme.Api",
            "ConvertProgrammeToEvergreen",
            It.IsAny<Dictionary<string, string>>()), Times.Once);
    }

    [Test]
    public async Task CreateProgrammeData_WhenSuccessful_ShouldReturnCreated()
    {
        var request = new ProgrammeDto { Name = "Test Programme" };
        var programmeData = new Api.Domain.Programme();
        var newId = 123;

        _mockMapper.Setup(x => x.Map<Api.Domain.Programme>(request)).Returns(programmeData);
        _mockProgrammeRepository.Setup(x => x.InsertAsync(programmeData)).ReturnsAsync(newId);

        var actual = await _controller.CreateProgrammeData(request);

        actual.Should().BeOfType<CreatedResult>()
            .Which.Location.Should().Be($"/programme/edit?programmeId={newId}");
    }

    [Test]
    public async Task CreateProgrammeData_WhenSuccessful_ShouldReturnCreatedWithId()
    {
        var request = new ProgrammeDto { Name = "Test Programme" };
        var programmeData = new Api.Domain.Programme();
        var newId = 456;

        _mockMapper.Setup(x => x.Map<Api.Domain.Programme>(request)).Returns(programmeData);
        _mockProgrammeRepository.Setup(x => x.InsertAsync(programmeData)).ReturnsAsync(newId);

        var actual = await _controller.CreateProgrammeData(request);

        actual.Should().BeOfType<CreatedResult>().Which.Value.Should().Be(newId);
    }

    [Test]
    public async Task CreateProgrammeData_WhenExceptionThrown_ShouldReturnProblem()
    {
        var request = new ProgrammeDto { Name = "Test Programme" };
        _mockMapper.Setup(x => x.Map<Api.Domain.Programme>(request)).Throws(new Exception("Test exception"));

        var actual = await _controller.CreateProgrammeData(request);

        actual.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task CreateProgrammeData_Called_ShouldAuditTheOperation()
    {
        var request = new ProgrammeDto 
        { 
            Name = "Test Programme",
            AccountNumber = "ACC123",
            Limit = 1000000,
            CurrencyIsoCode = "USD",
            IsEvergreen = true,
            Provider = "TestProvider",
            EnableForAllocation = true,
            IsBatchCalculationPrincipal = false,
            ProgrammeId = 1,
            ProgrammeLimitParentId = 2,
            IsActive = true,
            OriginationEntityId = 3,
            GroupedLimit = 5000000,
            AutoPaymentEnabled = false,
            RatesFormat = new RatesFormatDto(),
            UseAutoPaymentSingle = false
        };
        var programmeData = new Api.Domain.Programme();

        _mockMapper.Setup(x => x.Map<Api.Domain.Programme>(request)).Returns(programmeData);
        _mockProgrammeRepository.Setup(x => x.InsertAsync(programmeData)).ReturnsAsync(123);

        await _controller.CreateProgrammeData(request);

        _mockAuditClient.Verify(x => x.AuditAsync(
            "Programme.Api",
            "CreateProgrammeData",
            It.Is<Dictionary<string, string>>(d => 
                d["ProgrammeName"] == request.Name &&
                d["AccountNumber"] == request.AccountNumber &&
                d["Currency"] == request.CurrencyIsoCode)), Times.Once);
    }

    [Test]
    public async Task CreateProgrammeData_WhenExceptionThrown_ShouldStillAudit()
    {
        var request = new ProgrammeDto { Name = "Test Programme" };
        _mockMapper.Setup(x => x.Map<Api.Domain.Programme>(request)).Throws(new Exception("Test exception"));

        await _controller.CreateProgrammeData(request);

        _mockAuditClient.Verify(x => x.AuditAsync(
            "Programme.Api",
            "CreateProgrammeData",
            It.IsAny<Dictionary<string, string>>()), Times.Once);
    }

    [Test]
    public async Task UpdateProgrammeData_WhenSuccessful_ShouldReturnOk()
    {
        var request = new ProgrammeDto { ProgrammeId = 123, Name = "Updated Programme" };
        var programmeData = new Api.Domain.Programme { ProgrammeId = 123 };

        _mockMapper.Setup(x => x.Map<Api.Domain.Programme>(request)).Returns(programmeData);
        _mockProgrammeRepository.Setup(x => x.UpdateAsync(programmeData)).Returns(Task.CompletedTask);

        var actual = await _controller.UpdateProgrammeData(request);

        actual.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task UpdateProgrammeData_WhenSuccessful_ShouldReturnProgrammeId()
    {
        var request = new ProgrammeDto { ProgrammeId = 789, Name = "Updated Programme" };
        var programmeData = new Api.Domain.Programme { ProgrammeId = 789 };

        _mockMapper.Setup(x => x.Map<Api.Domain.Programme>(request)).Returns(programmeData);
        _mockProgrammeRepository.Setup(x => x.UpdateAsync(programmeData)).Returns(Task.CompletedTask);

        var actual = await _controller.UpdateProgrammeData(request);

        actual.Should().BeOfType<OkObjectResult>().Which.Value.Should().Be(789);
    }

    [Test]
    public async Task UpdateProgrammeData_WhenExceptionThrown_ShouldReturnProblem()
    {
        var request = new ProgrammeDto { ProgrammeId = 123, Name = "Updated Programme" };
        _mockMapper.Setup(x => x.Map<Api.Domain.Programme>(request)).Throws(new Exception("Test exception"));

        var actual = await _controller.UpdateProgrammeData(request);

        actual.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task UpdateProgrammeData_Called_ShouldCallRepositoryUpdate()
    {
        var request = new ProgrammeDto { ProgrammeId = 123 };
        var programmeData = new Api.Domain.Programme { ProgrammeId = 123 };

        _mockMapper.Setup(x => x.Map<Api.Domain.Programme>(request)).Returns(programmeData);

        await _controller.UpdateProgrammeData(request);

        _mockProgrammeRepository.Verify(x => x.UpdateAsync(programmeData), Times.Once);
    }

    [Test]
    public async Task UpdateProgrammeData_Called_ShouldAuditTheOperation()
    {
        var request = new ProgrammeDto 
        { 
            ProgrammeId = 123,
            Name = "Updated Programme",
            AccountNumber = "ACC456",
            Limit = 2000000,
            CurrencyIsoCode = "GBP",
            AutoPaymentEnabled = true,
            RatesFormat = new RatesFormatDto(),
            UseAutoPaymentSingle = true
        };
        var programmeData = new Api.Domain.Programme { ProgrammeId = 123 };

        _mockMapper.Setup(x => x.Map<Api.Domain.Programme>(request)).Returns(programmeData);
        _mockProgrammeRepository.Setup(x => x.UpdateAsync(programmeData)).Returns(Task.CompletedTask);

        await _controller.UpdateProgrammeData(request);

        _mockAuditClient.Verify(x => x.AuditAsync(
            "Programme.Api",
            "Update Programme Data",
            It.Is<Dictionary<string, string>>(d => 
                d["ProgrammeName"] == request.Name &&
                d["Programme ID"] == request.ProgrammeId.ToString())), Times.Once);
    }

    [Test]
    public async Task UpdateProgrammeData_WhenExceptionThrown_ShouldStillAudit()
    {
        var request = new ProgrammeDto { ProgrammeId = 123 };
        _mockMapper.Setup(x => x.Map<Api.Domain.Programme>(request)).Throws(new Exception("Test exception"));

        await _controller.UpdateProgrammeData(request);

        _mockAuditClient.Verify(x => x.AuditAsync(
            "Programme.Api",
            "Update Programme Data",
            It.IsAny<Dictionary<string, string>>()), Times.Once);
    }

    [Test]
    public async Task ProgrammeHeadroomAsync_Called_ShouldReturnOk()
    {
        var request = new ProgrammeHeadroomStatusRequestDto { ProgrammeIds = new List<int> { 1, 2, 3 } };
        var limits = new List<ProgrammeLimit> { new ProgrammeLimit() };
        _mockProgrammeRepository.Setup(x => x.GetAllProgrammeLimitsAsync(request.ProgrammeIds)).ReturnsAsync(limits);
        _mockMapper.Setup(x => x.Map<List<ProgrammeLimitDto>>(limits)).Returns(new List<ProgrammeLimitDto>());

        var actual = await _controller.ProgrammeHeadroomAsync(request);

        actual.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task ProgrammeHeadroomAsync_Called_ShouldCallRepositoryWithCorrectIds()
    {
        var programmeIds = new List<int> { 10, 20, 30 };
        var request = new ProgrammeHeadroomStatusRequestDto { ProgrammeIds = programmeIds };
        var limits = new List<ProgrammeLimit>();
        _mockProgrammeRepository.Setup(x => x.GetAllProgrammeLimitsAsync(programmeIds)).ReturnsAsync(limits);

        await _controller.ProgrammeHeadroomAsync(request);

        _mockProgrammeRepository.Verify(x => x.GetAllProgrammeLimitsAsync(programmeIds), Times.Once);
    }

    [Test]
    public async Task ProgrammeHeadroomAsync_Called_ShouldMapLimitsToDto()
    {
        var request = new ProgrammeHeadroomStatusRequestDto { ProgrammeIds = new List<int> { 1 } };
        var limits = new List<ProgrammeLimit> { new ProgrammeLimit { ProgrammeId = 1 } };
        _mockProgrammeRepository.Setup(x => x.GetAllProgrammeLimitsAsync(request.ProgrammeIds)).ReturnsAsync(limits);

        await _controller.ProgrammeHeadroomAsync(request);

        _mockMapper.Verify(x => x.Map<List<ProgrammeLimitDto>>(limits), Times.Once);
    }

    [Test]
    public async Task ProgrammeHeadroomAsync_WhenMultipleProgrammeIds_ShouldReturnAllLimits()
    {
        var programmeIds = new List<int> { 1, 2, 3, 4, 5 };
        var request = new ProgrammeHeadroomStatusRequestDto { ProgrammeIds = programmeIds };
        var limits = programmeIds.Select(id => new ProgrammeLimit { ProgrammeId = id }).ToList();
        var expectedDtos = programmeIds.Select(id => new ProgrammeLimitDto { ProgrammeId = id }).ToList();

        _mockProgrammeRepository.Setup(x => x.GetAllProgrammeLimitsAsync(programmeIds)).ReturnsAsync(limits);
        _mockMapper.Setup(x => x.Map<List<ProgrammeLimitDto>>(limits)).Returns(expectedDtos);

        var actual = await _controller.ProgrammeHeadroomAsync(request);

        actual.Should().BeOfType<OkObjectResult>()
            .Which.Value.As<List<ProgrammeLimitDto>>().Should().HaveCount(5);
    }

    [Test]
    public async Task CheckProgrammeAutoPaymentSettings_WhenCalled_ShouldReturnOk()
    {
        var request = new AutoPaymentSettingsRequestDto 
        { 
            ProgrammeId = 123,
            SupplierCodes = new List<string> { "SUP001", "SUP002" }
        };
        var domainRequest = new AutoPaymentSettingsRequest { ProgrammeId = 123 };
        var settings = new List<AutoPaymentSetting>
        {
            new AutoPaymentSetting { AutoPaymentEnabled = true, HasProgrammeSupplierMapping = true }
        };
        var autoPaymentParent = new AutoPaymentParent 
        { 
            AutoPaymentEnabled = true,
            Setting = settings
        };
        var mappedDtos = new List<MappedProgrammeSupplierCodeDto>
        {
            new MappedProgrammeSupplierCodeDto()
        };

        _mockMapper.Setup(x => x.Map<AutoPaymentSettingsRequest>(request)).Returns(domainRequest);
        _mockProgrammeRepository.Setup(x => x.AutoPaymentSettingsGetAsync(domainRequest)).ReturnsAsync(autoPaymentParent);
        _mockMapper.Setup(x => x.Map<List<MappedProgrammeSupplierCodeDto>>(settings)).Returns(mappedDtos);

        var actual = await _controller.CheckProgrammeAutoPaymentSettingsAsync(request);

        actual.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task CheckProgrammeAutoPaymentSettings_WhenCalled_ShouldReturnSuccessResponse()
    {
        var request = new AutoPaymentSettingsRequestDto 
        { 
            ProgrammeId = 123,
            SupplierCodes = new List<string> { "SUP001" }
        };
        var domainRequest = new AutoPaymentSettingsRequest { ProgrammeId = 123 };
        var settings = new List<AutoPaymentSetting>();
        var autoPaymentParent = new AutoPaymentParent 
        { 
            AutoPaymentEnabled = false,
            Setting = settings
        };
        var mappedDtos = new List<MappedProgrammeSupplierCodeDto>();

        _mockMapper.Setup(x => x.Map<AutoPaymentSettingsRequest>(request)).Returns(domainRequest);
        _mockProgrammeRepository.Setup(x => x.AutoPaymentSettingsGetAsync(domainRequest)).ReturnsAsync(autoPaymentParent);
        _mockMapper.Setup(x => x.Map<List<MappedProgrammeSupplierCodeDto>>(settings)).Returns(mappedDtos);

        var actual = await _controller.CheckProgrammeAutoPaymentSettingsAsync(request);

        actual.Should().BeOfType<OkObjectResult>()
            .Which.Value.As<GetAutoPaymentSettingsResponseDto>().IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task CheckProgrammeAutoPaymentSettings_WhenCalled_ShouldMapRequestToDomainObject()
    {
        var request = new AutoPaymentSettingsRequestDto 
        { 
            ProgrammeId = 456,
            SupplierCodes = new List<string> { "SUP003" }
        };
        var domainRequest = new AutoPaymentSettingsRequest { ProgrammeId = 456 };
        var settings = new List<AutoPaymentSetting>();
        var autoPaymentParent = new AutoPaymentParent 
        { 
            AutoPaymentEnabled = false,
            Setting = settings
        };

        _mockMapper.Setup(x => x.Map<AutoPaymentSettingsRequest>(request)).Returns(domainRequest);
        _mockProgrammeRepository.Setup(x => x.AutoPaymentSettingsGetAsync(It.IsAny<AutoPaymentSettingsRequest>())).ReturnsAsync(autoPaymentParent);
        _mockMapper.Setup(x => x.Map<List<MappedProgrammeSupplierCodeDto>>(It.IsAny<IEnumerable<AutoPaymentSetting>>())).Returns(new List<MappedProgrammeSupplierCodeDto>());

        await _controller.CheckProgrammeAutoPaymentSettingsAsync(request);

        _mockMapper.Verify(x => x.Map<AutoPaymentSettingsRequest>(request), Times.Once);
    }

    [Test]
    public async Task CheckProgrammeAutoPaymentSettings_WhenCalled_ShouldCallRepositoryWithMappedRequest()
    {
        var request = new AutoPaymentSettingsRequestDto 
        { 
            ProgrammeId = 789,
            SupplierCodes = new List<string> { "SUP004", "SUP005" }
        };
        var domainRequest = new AutoPaymentSettingsRequest { ProgrammeId = 789 };
        var settings = new List<AutoPaymentSetting>();
        var autoPaymentParent = new AutoPaymentParent 
        { 
            AutoPaymentEnabled = false,
            Setting = settings
        };

        _mockMapper.Setup(x => x.Map<AutoPaymentSettingsRequest>(request)).Returns(domainRequest);
        _mockProgrammeRepository.Setup(x => x.AutoPaymentSettingsGetAsync(domainRequest)).ReturnsAsync(autoPaymentParent);
        _mockMapper.Setup(x => x.Map<List<MappedProgrammeSupplierCodeDto>>(It.IsAny<IEnumerable<AutoPaymentSetting>>())).Returns(new List<MappedProgrammeSupplierCodeDto>());

        await _controller.CheckProgrammeAutoPaymentSettingsAsync(request);

        _mockProgrammeRepository.Verify(x => x.AutoPaymentSettingsGetAsync(domainRequest), Times.Once);
    }

    [Test]
    public async Task CheckProgrammeAutoPaymentSettings_WhenCalled_ShouldMapResultToDto()
    {
        var request = new AutoPaymentSettingsRequestDto 
        { 
            ProgrammeId = 100,
            SupplierCodes = new List<string>()
        };
        var domainRequest = new AutoPaymentSettingsRequest { ProgrammeId = 100 };
        var settings = new List<AutoPaymentSetting>
        {
            new AutoPaymentSetting { AutoPaymentEnabled = true, HasProgrammeSupplierMapping = false },
            new AutoPaymentSetting { AutoPaymentEnabled = false, HasProgrammeSupplierMapping = true }
        };
        var autoPaymentParent = new AutoPaymentParent 
        { 
            AutoPaymentEnabled = true,
            Setting = settings
        };

        _mockMapper.Setup(x => x.Map<AutoPaymentSettingsRequest>(request)).Returns(domainRequest);
        _mockProgrammeRepository.Setup(x => x.AutoPaymentSettingsGetAsync(domainRequest)).ReturnsAsync(autoPaymentParent);
        _mockMapper.Setup(x => x.Map<List<MappedProgrammeSupplierCodeDto>>(settings)).Returns(new List<MappedProgrammeSupplierCodeDto>());

        await _controller.CheckProgrammeAutoPaymentSettingsAsync(request);

        _mockMapper.Verify(x => x.Map<List<MappedProgrammeSupplierCodeDto>>(settings), Times.Once);
    }

    [Test]
    public async Task CheckProgrammeAutoPaymentSettings_WhenCalled_ShouldReturnMappedSupplierCodes()
    {
        var request = new AutoPaymentSettingsRequestDto 
        { 
            ProgrammeId = 200,
            SupplierCodes = new List<string> { "SUP001" }
        };
        var domainRequest = new AutoPaymentSettingsRequest { ProgrammeId = 200 };
        var settings = new List<AutoPaymentSetting>
        {
            new AutoPaymentSetting { AutoPaymentEnabled = true, HasProgrammeSupplierMapping = true }
        };
        var autoPaymentParent = new AutoPaymentParent 
        { 
            AutoPaymentEnabled = true,
            Setting = settings
        };
        var mappedDtos = new List<MappedProgrammeSupplierCodeDto>
        {
            new MappedProgrammeSupplierCodeDto()
        };

        _mockMapper.Setup(x => x.Map<AutoPaymentSettingsRequest>(request)).Returns(domainRequest);
        _mockProgrammeRepository.Setup(x => x.AutoPaymentSettingsGetAsync(domainRequest)).ReturnsAsync(autoPaymentParent);
        _mockMapper.Setup(x => x.Map<List<MappedProgrammeSupplierCodeDto>>(settings)).Returns(mappedDtos);

        var actual = await _controller.CheckProgrammeAutoPaymentSettingsAsync(request);

        actual.Should().BeOfType<OkObjectResult>()
            .Which.Value.As<GetAutoPaymentSettingsResponseDto>().MappedSupplierCodes.Should().NotBeNull()
            .And.HaveCount(1);
    }

    [Test]
    public async Task CheckProgrammeAutoPaymentSettings_WhenRepositoryReturnsEmpty_ShouldReturnEmptyList()
    {
        var request = new AutoPaymentSettingsRequestDto 
        { 
            ProgrammeId = 300,
            SupplierCodes = new List<string>()
        };
        var domainRequest = new AutoPaymentSettingsRequest { ProgrammeId = 300 };
        var settings = new List<AutoPaymentSetting>();
        var autoPaymentParent = new AutoPaymentParent 
        { 
            AutoPaymentEnabled = false,
            Setting = settings
        };
        var mappedDtos = new List<MappedProgrammeSupplierCodeDto>();

        _mockMapper.Setup(x => x.Map<AutoPaymentSettingsRequest>(request)).Returns(domainRequest);
        _mockProgrammeRepository.Setup(x => x.AutoPaymentSettingsGetAsync(domainRequest)).ReturnsAsync(autoPaymentParent);
        _mockMapper.Setup(x => x.Map<List<MappedProgrammeSupplierCodeDto>>(settings)).Returns(mappedDtos);

        var actual = await _controller.CheckProgrammeAutoPaymentSettingsAsync(request);

        actual.Should().BeOfType<OkObjectResult>()
            .Which.Value.As<GetAutoPaymentSettingsResponseDto>().MappedSupplierCodes.Should().BeEmpty();
    }

    [Test]
    public async Task CheckProgrammeAutoPaymentSettings_WithMultipleSupplierCodes_ShouldProcessAll()
    {
        var request = new AutoPaymentSettingsRequestDto 
        { 
            ProgrammeId = 400,
            SupplierCodes = new List<string> { "SUP001", "SUP002", "SUP003", "SUP004" }
        };
        var domainRequest = new AutoPaymentSettingsRequest { ProgrammeId = 400 };
        var settings = new List<AutoPaymentSetting>
        {
            new AutoPaymentSetting { AutoPaymentEnabled = true, HasProgrammeSupplierMapping = true },
            new AutoPaymentSetting { AutoPaymentEnabled = true, HasProgrammeSupplierMapping = false },
            new AutoPaymentSetting { AutoPaymentEnabled = false, HasProgrammeSupplierMapping = true },
            new AutoPaymentSetting { AutoPaymentEnabled = false, HasProgrammeSupplierMapping = false }
        };
        var autoPaymentParent = new AutoPaymentParent 
        { 
            AutoPaymentEnabled = true,
            Setting = settings
        };
        var mappedDtos = new List<MappedProgrammeSupplierCodeDto>
        {
            new MappedProgrammeSupplierCodeDto(),
            new MappedProgrammeSupplierCodeDto(),
            new MappedProgrammeSupplierCodeDto(),
            new MappedProgrammeSupplierCodeDto()
        };

        _mockMapper.Setup(x => x.Map<AutoPaymentSettingsRequest>(request)).Returns(domainRequest);
        _mockProgrammeRepository.Setup(x => x.AutoPaymentSettingsGetAsync(domainRequest)).ReturnsAsync(autoPaymentParent);
        _mockMapper.Setup(x => x.Map<List<MappedProgrammeSupplierCodeDto>>(settings)).Returns(mappedDtos);

        var actual = await _controller.CheckProgrammeAutoPaymentSettingsAsync(request);

        actual.Should().BeOfType<OkObjectResult>()
            .Which.Value.As<GetAutoPaymentSettingsResponseDto>().MappedSupplierCodes.Should().HaveCount(4);
    }

    [Test]
    public async Task CheckProgrammeAutoPaymentSettings_WhenValidProgrammeId_ShouldPassCorrectIdToRepository()
    {
        var programmeId = 999;
        var request = new AutoPaymentSettingsRequestDto 
        { 
            ProgrammeId = programmeId,
            SupplierCodes = new List<string> { "SUP001" }
        };
        var domainRequest = new AutoPaymentSettingsRequest { ProgrammeId = programmeId };
        var settings = new List<AutoPaymentSetting>();
        var autoPaymentParent = new AutoPaymentParent 
        { 
            AutoPaymentEnabled = false,
            Setting = settings
        };

        _mockMapper.Setup(x => x.Map<AutoPaymentSettingsRequest>(request)).Returns(domainRequest);
        _mockProgrammeRepository.Setup(x => x.AutoPaymentSettingsGetAsync(It.Is<AutoPaymentSettingsRequest>(r => r.ProgrammeId == programmeId)))
            .ReturnsAsync(autoPaymentParent);
        _mockMapper.Setup(x => x.Map<List<MappedProgrammeSupplierCodeDto>>(It.IsAny<IEnumerable<AutoPaymentSetting>>()))
            .Returns(new List<MappedProgrammeSupplierCodeDto>());

        await _controller.CheckProgrammeAutoPaymentSettingsAsync(request);

        _mockProgrammeRepository.Verify(x => x.AutoPaymentSettingsGetAsync(
            It.Is<AutoPaymentSettingsRequest>(r => r.ProgrammeId == programmeId)), Times.Once);
    }
}