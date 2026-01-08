namespace WayForPaySDK.Exceptions;

/// <summary>
/// Base exception for all WayForPay SDK errors.
/// </summary>
public class WayForPayException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WayForPayException"/> class.
    /// </summary>
    public WayForPayException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WayForPayException"/> class with a message.
    /// </summary>
    public WayForPayException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WayForPayException"/> class with a message and inner exception.
    /// </summary>
    public WayForPayException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
