namespace WayForPaySDK.Exceptions;

/// <summary>
/// Exception thrown when JSON parsing fails.
/// </summary>
public sealed class JsonParseException : WayForPayException
{
    /// <summary>
    /// Gets the raw JSON content that failed to parse.
    /// </summary>
    public string? RawContent { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonParseException"/> class.
    /// </summary>
    public JsonParseException() : base("Failed to parse JSON response.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonParseException"/> class with a message.
    /// </summary>
    public JsonParseException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonParseException"/> class with raw content.
    /// </summary>
    public JsonParseException(string message, string? rawContent)
        : base(message)
    {
        RawContent = rawContent;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonParseException"/> class with a message and inner exception.
    /// </summary>
    public JsonParseException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonParseException"/> class with raw content and inner exception.
    /// </summary>
    public JsonParseException(string message, string? rawContent, Exception innerException)
        : base(message, innerException)
    {
        RawContent = rawContent;
    }
}
