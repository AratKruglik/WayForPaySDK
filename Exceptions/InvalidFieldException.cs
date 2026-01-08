namespace WayForPaySDK.Exceptions;

/// <summary>
/// Exception thrown when a field value is invalid.
/// </summary>
public sealed class InvalidFieldException : WayForPayException
{
    /// <summary>
    /// Gets the name of the invalid field.
    /// </summary>
    public string? FieldName { get; }

    /// <summary>
    /// Gets the invalid value that was provided.
    /// </summary>
    public object? FieldValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidFieldException"/> class.
    /// </summary>
    public InvalidFieldException() : base("A field value is invalid.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidFieldException"/> class with a message.
    /// </summary>
    public InvalidFieldException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidFieldException"/> class with field details.
    /// </summary>
    public InvalidFieldException(string fieldName, object? fieldValue, string? message = null)
        : base(message ?? $"Invalid value for field '{fieldName}': {fieldValue}")
    {
        FieldName = fieldName;
        FieldValue = fieldValue;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidFieldException"/> class with a message and inner exception.
    /// </summary>
    public InvalidFieldException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
