namespace Programme.ApiClient;

public interface IProgrammeApiConfig
{
    public string Server { get; }

    public string BaseUrl { get; }

    public string Scope { get; }
}