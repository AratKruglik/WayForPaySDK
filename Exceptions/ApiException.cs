namespace WayForPaySDK.Exceptions;

/// <summary>
/// Exception thrown when the WayForPay API returns an error.
/// </summary>
public sealed class ApiException : WayForPayException
{
    /// <summary>
    /// Gets the reason code returned by the API.
    /// </summary>
    public int? ReasonCode { get; }

    /// <summary>
    /// Gets the reason message returned by the API.
    /// </summary>
    public string? Reason { get; }

    /// <summary>
    /// Gets the HTTP status code if available.
    /// </summary>
    public int? HttpStatusCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiException"/> class.
    /// </summary>
    public ApiException() : base("An API error occurred.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiException"/> class with a message.
    /// </summary>
    public ApiException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiException"/> class with API error details.
    /// </summary>
    public ApiException(int reasonCode, string reason)
        : base($"WayForPay API error [{reasonCode}]: {reason}")
    {
        ReasonCode = reasonCode;
        Reason = reason;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiException"/> class with API error details and HTTP status.
    /// </summary>
    public ApiException(int reasonCode, string reason, int httpStatusCode)
        : base($"WayForPay API error [{reasonCode}]: {reason} (HTTP {httpStatusCode})")
    {
        ReasonCode = reasonCode;
        Reason = reason;
        HttpStatusCode = httpStatusCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiException"/> class with a message and inner exception.
    /// </summary>
    public ApiException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
