using System;
using System.Collections.Generic;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Programme.Api;

public static class SwaggerRegistration
{
    public static void ConfigureVersionedSwagger(this IServiceCollection serviceCollection, ConfigurationManager configuration)
    {
        serviceCollection.AddEndpointsApiExplorer();
        serviceCollection.AddSwaggerGen(c =>
        {
            var azureAdBaseUrl = configuration["AzureAd:Instance"];
            var tenantId = configuration["AzureAd:TenantId"];
            var scope = configuration["SwaggerAuthentication:Scope"];

            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{azureAdBaseUrl}{tenantId}/oauth2/v2.0/authorize"),
                        TokenUrl = new Uri($"{azureAdBaseUrl}{tenantId}/oauth2/v2.0/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { scope, "Access As User" }
                        }
                    }
                }
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" },
                        Scheme = "oauth2",
                        Name = "oauth2",
                        In = ParameterLocation.Header
                    },
                    new[] { scope }
                }
            });
        });

        serviceCollection.ConfigureOptions<SwaggerConfigOptions>();

        serviceCollection.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("x-api-version"),
                new MediaTypeApiVersionReader("x-api-version"));
        }).AddApiExplorer(config =>
        {
            config.GroupNameFormat = "'v'VVV";
            config.SubstituteApiVersionInUrl = true;
        });
    }

    public static void UseVersionedSwagger(this WebApplication app, ConfigurationManager configuration)
    {
        app.UseSwagger(c =>
        {
            c.RouteTemplate = "api/swagger/{documentname}/swagger.json";
        });


        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = "api/swagger";

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint($"/api/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToLower());

                options.OAuthClientId(configuration["SwaggerAuthentication:ClientId"]);
                options.OAuthClientSecret(configuration["SwaggerAuthentication:ClientSecret"]);
                options.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            }
        });
    }
}