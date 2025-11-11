using Microsoft.Extensions.Configuration;

namespace Programme.ApiClient;

public class ProgrammeApiConfig : IProgrammeApiConfig
{
    public ProgrammeApiConfig(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        configuration.Bind("ProgrammeApi", this);
    }

    public string Server { get; set; }

    public string BaseUrl { get; set; }

    public string Scope { get; set; }
}