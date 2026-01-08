namespace WayForPaySDK.Domain;

/// <summary>
/// Represents a payment card.
/// </summary>
public sealed record Card
{
    /// <summary>
    /// Gets the card number (PAN).
    /// </summary>
    public required string Number { get; init; }

    /// <summary>
    /// Gets the card expiration month (1-12).
    /// </summary>
    public required int ExpireMonth { get; init; }

    /// <summary>
    /// Gets the card expiration year (4 digits, e.g., 2025).
    /// </summary>
    public required int ExpireYear { get; init; }

    /// <summary>
    /// Gets the card CVV/CVC code.
    /// </summary>
    public required string Cvv { get; init; }

    /// <summary>
    /// Gets the cardholder name.
    /// </summary>
    public required string Holder { get; init; }
}
