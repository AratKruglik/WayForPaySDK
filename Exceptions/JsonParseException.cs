namespace WayForPaySDK.Exceptions;

/// <summary>
/// Thrown when JSON parsing fails.
/// </summary>
public sealed class JsonParseException : WayForPayException
{
    public string? RawContent { get; }

    public JsonParseException() : base("Failed to parse JSON response.")
    {
    }

    public JsonParseException(string message) : base(message)
    {
    }

    public JsonParseException(string message, string? rawContent)
        : base(message)
    {
        RawContent = rawContent;
    }

    public JsonParseException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public JsonParseException(string message, string? rawContent, Exception innerException)
        : base(message, innerException)
    {
        RawContent = rawContent;
    }
}
