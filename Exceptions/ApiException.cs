namespace WayForPaySDK.Exceptions;

/// <summary>
/// Thrown when the WayForPay API returns an error.
/// </summary>
public sealed class ApiException : WayForPayException
{
    public int? ReasonCode { get; }
    public string? Reason { get; }
    public int? HttpStatusCode { get; }

    public ApiException() : base("An API error occurred.")
    {
    }

    public ApiException(string message) : base(message)
    {
    }

    public ApiException(int reasonCode, string reason)
        : base($"WayForPay API error [{reasonCode}]: {reason}")
    {
        ReasonCode = reasonCode;
        Reason = reason;
    }

    public ApiException(int reasonCode, string reason, int httpStatusCode)
        : base($"WayForPay API error [{reasonCode}]: {reason} (HTTP {httpStatusCode})")
    {
        ReasonCode = reasonCode;
        Reason = reason;
        HttpStatusCode = httpStatusCode;
    }

    public ApiException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
