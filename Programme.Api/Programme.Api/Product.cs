namespace Programme.Api;

public class Product
{
    public string Title { get; set; }

    public string Version { get; set; }

    public string GitCommit { get; set; }

    public override string ToString() => $"{Title} v{Version}";
}