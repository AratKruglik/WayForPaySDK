namespace WayForPaySDK.Http;

internal static class ApiUrlBuilder
{
    internal static Uri BuildAlternateUrl(string apiBaseUrl, string relativePath)
    {
        var baseUri = new Uri(apiBaseUrl);
        var builder = new UriBuilder(baseUri);

        if (builder.Path.EndsWith("/api", StringComparison.OrdinalIgnoreCase))
        {
            builder.Path = builder.Path[..^4] + relativePath;
        }
        else if (builder.Path.EndsWith("/api/", StringComparison.OrdinalIgnoreCase))
        {
            builder.Path = builder.Path[..^5] + relativePath;
        }
        else
        {
            builder.Path = builder.Path.TrimEnd('/') + relativePath;
        }

        return builder.Uri;
    }
}
