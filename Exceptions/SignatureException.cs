namespace WayForPaySDK.Exceptions;

/// <summary>
/// Exception thrown when signature validation fails.
/// </summary>
public sealed class SignatureException : WayForPayException
{
    /// <summary>
    /// Gets the expected signature value.
    /// </summary>
    public string? ExpectedSignature { get; }

    /// <summary>
    /// Gets the actual signature value received.
    /// </summary>
    public string? ActualSignature { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SignatureException"/> class.
    /// </summary>
    public SignatureException() : base("Signature validation failed.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SignatureException"/> class with a message.
    /// </summary>
    public SignatureException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SignatureException"/> class with signature details.
    /// </summary>
    public SignatureException(string? expectedSignature, string? actualSignature)
        : base("Signature validation failed. The response signature does not match the expected value.")
    {
        ExpectedSignature = expectedSignature;
        ActualSignature = actualSignature;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SignatureException"/> class with a message and inner exception.
    /// </summary>
    public SignatureException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
