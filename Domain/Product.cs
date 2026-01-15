using WayForPaySDK.Exceptions;

namespace WayForPaySDK.Domain;

/// <summary>
/// Represents a product in a payment transaction.
/// </summary>
public sealed record Product
{
    /// <summary>
    /// The product name. Required and must not be empty.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The product price. Must be greater than zero.
    /// </summary>
    public required decimal Price { get; init; }

    /// <summary>
    /// The product count/quantity. Must be greater than zero.
    /// </summary>
    public required int Count { get; init; }

    /// <summary>
    /// Validates the product data and returns any validation errors.
    /// </summary>
    /// <returns>A list of validation error messages. Empty if valid.</returns>
    public IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
        {
            errors.Add("Product name is required.");
        }
        else if (Name.Length > 1000)
        {
            errors.Add("Product name is too long (max 1000 characters).");
        }

        if (Price <= 0)
        {
            errors.Add("Product price must be greater than zero.");
        }

        if (Count <= 0)
        {
            errors.Add("Product count must be greater than zero.");
        }

        return errors;
    }

    public void ValidateAndThrow()
    {
        var errors = Validate();
        if (errors.Count > 0)
        {
            throw new ValidationException("Product validation failed.", errors);
        }
    }

    public bool IsValid => Validate().Count == 0;
}
