using WayForPaySDK.Exceptions;

namespace WayForPaySDK.Domain;

/// <summary>
/// Represents payment card data for card-present transactions.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Security Note:</strong> Card data should never be logged or stored persistently.
/// This object should only exist in memory for the duration of a transaction.
/// Consider using tokenization (RecToken) for recurring payments instead.
/// </para>
/// </remarks>
public sealed record Card
{
    /// <summary>
    /// The card number (PAN). Must be 13-19 digits.
    /// </summary>
    public required string Number { get; init; }

    /// <summary>
    /// Expiration month (1-12).
    /// </summary>
    public required int ExpireMonth { get; init; }

    /// <summary>
    /// Expiration year (4-digit format, e.g., 2025).
    /// </summary>
    public required int ExpireYear { get; init; }

    /// <summary>
    /// Card verification value (CVV/CVC). Must be 3-4 digits.
    /// </summary>
    public required string Cvv { get; init; }

    /// <summary>
    /// Cardholder name as printed on the card.
    /// </summary>
    public required string Holder { get; init; }

    /// <summary>
    /// Validates the card data and returns any validation errors.
    /// </summary>
    /// <returns>A list of validation error messages. Empty if valid.</returns>
    public IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Number))
        {
            errors.Add("Card number is required.");
        }
        else
        {
            var digitsOnly = new string(Number.Where(char.IsDigit).ToArray());
            if (digitsOnly.Length < 13 || digitsOnly.Length > 19)
            {
                errors.Add("Card number must be between 13 and 19 digits.");
            }
            else if (!PassesLuhnCheck(digitsOnly))
            {
                errors.Add("Card number failed Luhn check.");
            }
        }

        if (string.IsNullOrWhiteSpace(Cvv))
        {
            errors.Add("CVV is required.");
        }
        else if (!Cvv.All(char.IsDigit) || Cvv.Length < 3 || Cvv.Length > 4)
        {
            errors.Add("CVV must be 3 or 4 digits.");
        }

        if (ExpireMonth < 1 || ExpireMonth > 12)
        {
            errors.Add("Expiration month must be between 1 and 12.");
        }

        var currentYear = DateTime.UtcNow.Year;
        if (ExpireYear < currentYear || ExpireYear > currentYear + 20)
        {
            errors.Add($"Expiration year must be between {currentYear} and {currentYear + 20}.");
        }

        if (ExpireMonth >= 1 && ExpireMonth <= 12 && ExpireYear >= currentYear)
        {
            var lastDayOfMonth = DateTime.DaysInMonth(ExpireYear, ExpireMonth);
            var expirationDate = new DateTime(ExpireYear, ExpireMonth, lastDayOfMonth);
            if (expirationDate < DateTime.UtcNow.Date)
            {
                errors.Add("Card has expired.");
            }
        }

        if (string.IsNullOrWhiteSpace(Holder))
        {
            errors.Add("Cardholder name is required.");
        }

        return errors;
    }

    /// <summary>
    /// Validates the card data and throws if invalid.
    /// </summary>
    /// <exception cref="ValidationException">Thrown when validation fails.</exception>
    public void ValidateAndThrow()
    {
        var errors = Validate();
        if (errors.Count > 0)
        {
            throw new ValidationException("Card validation failed.", errors);
        }
    }

    /// <summary>
    /// Returns whether the card data passes all validation checks.
    /// </summary>
    public bool IsValid => Validate().Count == 0;

    /// <summary>
    /// Validates a card number using the Luhn algorithm (mod 10).
    /// </summary>
    private static bool PassesLuhnCheck(string number)
    {
        var sum = 0;
        var alternate = false;

        for (var i = number.Length - 1; i >= 0; i--)
        {
            var digit = number[i] - '0';

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }
}
