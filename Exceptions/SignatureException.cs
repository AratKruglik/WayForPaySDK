namespace WayForPaySDK.Exceptions;

/// <summary>
/// Thrown when signature validation fails.
/// </summary>
public sealed class SignatureException : WayForPayException
{
    public string? ExpectedSignature { get; }
    public string? ActualSignature { get; }

    public SignatureException() : base("Signature validation failed.")
    {
    }

    public SignatureException(string message) : base(message)
    {
    }

    public SignatureException(string? expectedSignature, string? actualSignature)
        : base("Signature validation failed. The response signature does not match the expected value.")
    {
        ExpectedSignature = expectedSignature;
        ActualSignature = actualSignature;
    }

    public SignatureException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
