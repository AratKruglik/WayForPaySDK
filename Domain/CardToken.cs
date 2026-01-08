namespace WayForPaySDK.Domain;

/// <summary>
/// Represents a tokenized card for recurring payments.
/// </summary>
public sealed record CardToken
{
    /// <summary>
    /// Gets the token value.
    /// </summary>
    public required string Token { get; init; }
}
