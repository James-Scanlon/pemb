using System;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Programme.Api;

public class SwaggerConfigOptions(IApiVersionDescriptionProvider provider) : IConfigureNamedOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                description.GroupName,
                CreateVersionInfo(description));
        }
    }

    public void Configure(string name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    private static OpenApiInfo CreateVersionInfo(ApiVersionDescription desc)
    {
        var product = ProductVersioning.GetProduct();

        var info = new OpenApiInfo
        {
            Title = $"{product.Title} Build {product.Version}",
            Version = desc.ApiVersion.ToString(),
            Contact = new OpenApiContact
            {
                Name = "Helpdesk",
                Email = "helpdesk@pembertonam.com",
                Url = new Uri("https://www.pembertonam.com"),
            },
            Description = string.IsNullOrEmpty(product.GitCommit)
                ? string.Empty
                : $"Built from Git commit ID {product.GitCommit}"
        };

        if (desc.IsDeprecated)
        {
            info.Description += "This API version has been deprecated.";
        }

        return info;
    }
}