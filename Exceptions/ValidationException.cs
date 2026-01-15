namespace WayForPaySDK.Exceptions;

/// <summary>
/// Thrown when validation of domain objects fails.
/// </summary>
public sealed class ValidationException : WayForPayException
{
    /// <summary>
    /// The list of validation errors.
    /// </summary>
    public IReadOnlyList<string> Errors { get; }

    public ValidationException(string message, IReadOnlyList<string> errors)
        : base(message)
    {
        Errors = errors ?? Array.Empty<string>();
    }

    public ValidationException(IReadOnlyList<string> errors)
        : base("Validation failed. See Errors property for details.")
    {
        Errors = errors ?? Array.Empty<string>();
    }
}
