namespace WayForPaySDK.Domain;

/// <summary>
/// Represents a product in an order.
/// </summary>
public sealed record Product
{
    /// <summary>
    /// Gets the product name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the product price.
    /// </summary>
    public required decimal Price { get; init; }

    /// <summary>
    /// Gets the product quantity.
    /// </summary>
    public required int Count { get; init; }
}
