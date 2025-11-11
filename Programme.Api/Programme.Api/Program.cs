using System;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using ParkSquare.ConfigDump.AspNetCore;
using Serilog;

namespace Programme.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var config = builder.Configuration;

        ConfigureServices(builder.Services, config);
        ConfigureHost(builder.Host);
        ConfigureApp(builder.Build(), config);
    }

    private static void ConfigureServices(IServiceCollection serviceCollection, ConfigurationManager configuration)
    {
        serviceCollection.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
        });

        serviceCollection.AddConfigDump();

        serviceCollection.AddMicrosoftIdentityWebApiAuthentication(configuration).EnableTokenAcquisitionToCallDownstreamApi().AddInMemoryTokenCaches();
        serviceCollection.AddAuthentication();

        serviceCollection.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        serviceCollection.ConfigureHealthChecks();

        serviceCollection.AddControllers();
        serviceCollection.ConfigureVersionedSwagger(configuration);
        serviceCollection.AddResponseCaching();
        serviceCollection.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        serviceCollection.AddApplicationInsightsTelemetry();
        serviceCollection.AddMemoryCache();

        serviceCollection.AddDependencies();
    }

    private static void ConfigureHost(IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((hostingContext, loggerConfiguration) =>
            loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));
    }

    private static void ConfigureApp(WebApplication app, ConfigurationManager configuration)
    {
        app.UseSerilogRequestLogging();
        app.UseRouting();

        if (app.Environment.IsDevelopment())
        {
            app.UseVersionedSwagger(configuration);
        }

        TestAutoMapper(app.Services);

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/healthz");

        app.Run();
    }

    private static void TestAutoMapper(IServiceProvider serviceProvider)
    {
        var service = serviceProvider.GetService<IMapper>();
        service?.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
