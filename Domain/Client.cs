namespace WayForPaySDK.Domain;

/// <summary>
/// Represents a client (customer) making a payment.
/// </summary>
public sealed record Client
{
    /// <summary>
    /// Gets the client's unique identifier.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Gets the client's first name.
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    /// Gets the client's last name.
    /// </summary>
    public string? LastName { get; init; }

    /// <summary>
    /// Gets the client's email address.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Gets the client's phone number.
    /// </summary>
    public string? Phone { get; init; }

    /// <summary>
    /// Gets the client's country code (ISO 3166-1 alpha-3).
    /// </summary>
    public string? Country { get; init; }

    /// <summary>
    /// Gets the client's IP address.
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    /// Gets the client's address.
    /// </summary>
    public string? Address { get; init; }

    /// <summary>
    /// Gets the client's city.
    /// </summary>
    public string? City { get; init; }

    /// <summary>
    /// Gets the client's state or region.
    /// </summary>
    public string? State { get; init; }

    /// <summary>
    /// Gets the client's postal/ZIP code.
    /// </summary>
    public string? ZipCode { get; init; }
}
