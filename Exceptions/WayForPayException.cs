namespace WayForPaySDK.Exceptions;

public class WayForPayException : Exception
{
    public WayForPayException()
    {
    }

    public WayForPayException(string message) : base(message)
    {
    }

    public WayForPayException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
