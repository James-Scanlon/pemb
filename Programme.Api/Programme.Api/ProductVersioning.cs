using System.Reflection;

namespace Programme.Api;

public static class ProductVersioning
{
    public static Product GetProduct()
    {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly == null) return null;

        var versionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        var version = versionAttribute == null ? "Unknown" : versionAttribute.InformationalVersion;

        var gitCommit = string.Empty;

        if (version.Contains('+'))
        {
            var parts = version.Split('+');
            version = parts[0];
            gitCommit = parts[1];
        }

        var productAttribute = assembly.GetCustomAttribute<AssemblyProductAttribute>();
        var product = productAttribute == null ? "Unknown" : productAttribute.Product;

        return new Product
        {
            Title = product,
            Version = version,
            GitCommit = gitCommit
        };
    }
}