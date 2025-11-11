using AutoMapper;
using NUnit.Framework;
using Programme.Api.Mapping;
using Programme.Api.Repositories;

namespace Programme.Api.Tests.Mapping;

public class AutoMapperTests
{
    [Test]
    public void AutoMapper_DtoMappingProfile_CheckIsConfiguredCorrectly()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        config.AssertConfigurationIsValid();
    }

    [Test]
    public void AutoMapper_DatabaseMappingProfile_CheckIsConfiguredCorrectly()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<DatabaseMappingProfile>());
        config.AssertConfigurationIsValid();
    }
}