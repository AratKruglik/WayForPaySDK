namespace WayForPaySDK.Exceptions;

/// <summary>
/// Thrown when a field value is invalid.
/// </summary>
public sealed class InvalidFieldException : WayForPayException
{
    public string? FieldName { get; }
    public object? FieldValue { get; }

    public InvalidFieldException() : base("A field value is invalid.")
    {
    }

    public InvalidFieldException(string message) : base(message)
    {
    }

    public InvalidFieldException(string fieldName, object? fieldValue, string? message = null)
        : base(message ?? $"Invalid value for field '{fieldName}': {fieldValue}")
    {
        FieldName = fieldName;
        FieldValue = fieldValue;
    }

    public InvalidFieldException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
