using System.Net;
using System.Text.RegularExpressions;
using WayForPaySDK.Exceptions;

namespace WayForPaySDK.Domain;

/// <summary>
/// Represents client (customer) information for payment transactions.
/// </summary>
public sealed partial record Client
{
    public string? Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Country { get; init; }
    public string? IpAddress { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? ZipCode { get; init; }

    /// <summary>
    /// Validates the client data and returns any validation errors.
    /// Only validates fields that have values - empty/null fields are allowed.
    /// </summary>
    /// <returns>A list of validation error messages. Empty if valid.</returns>
    public IReadOnlyList<string> Validate()
    {
        var errors = new List<string>();

        if (!string.IsNullOrEmpty(Email))
        {
            try
            {
                if (!EmailRegex().IsMatch(Email))
                {
                    errors.Add("Invalid email format.");
                }
            }
            catch (RegexMatchTimeoutException)
            {
                errors.Add("Email validation timed out - possibly malicious input.");
            }
        }

        if (!string.IsNullOrEmpty(Phone))
        {
            try
            {
                if (!PhoneRegex().IsMatch(Phone))
                {
                    errors.Add("Invalid phone format. Expected digits, spaces, dashes, parentheses, or plus sign.");
                }
            }
            catch (RegexMatchTimeoutException)
            {
                errors.Add("Phone validation timed out - possibly malicious input.");
            }
        }

        if (!string.IsNullOrEmpty(IpAddress))
        {
            if (!IPAddress.TryParse(IpAddress, out _))
            {
                errors.Add("Invalid IP address format.");
            }
        }

        if (!string.IsNullOrEmpty(Country) && Country.Length > 100)
        {
            errors.Add("Country name is too long (max 100 characters).");
        }

        if (!string.IsNullOrEmpty(FirstName) && FirstName.Length > 100)
        {
            errors.Add("First name is too long (max 100 characters).");
        }

        if (!string.IsNullOrEmpty(LastName) && LastName.Length > 100)
        {
            errors.Add("Last name is too long (max 100 characters).");
        }

        return errors;
    }

    public void ValidateAndThrow()
    {
        var errors = Validate();
        if (errors.Count > 0)
        {
            throw new ValidationException("Client validation failed.", errors);
        }
    }

    public bool IsValid => Validate().Count == 0;

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 250)]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"^[\d\s\-\+\(\)]{7,20}$", RegexOptions.None, matchTimeoutMilliseconds: 250)]
    private static partial Regex PhoneRegex();
}
