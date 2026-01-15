using FluentAssertions;
using WayForPaySDK.Domain;
using WayForPaySDK.Exceptions;

namespace WayForPaySDK.Tests.Unit;

public class CardValidationTests
{
    [Fact]
    public void Validate_WithValidCard_ReturnsEmptyList()
    {
        var card = new Card
        {
            Number = "4532015112830366",
            ExpireMonth = 12,
            ExpireYear = DateTime.UtcNow.Year + 1,
            Cvv = "123",
            Holder = "JOHN DOE"
        };

        var errors = card.Validate();

        errors.Should().BeEmpty();
        card.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("4532015112830366")]
    [InlineData("5425233430109903")]
    [InlineData("374245455400126")]
    [InlineData("6011000990139424")]
    public void Validate_WithValidLuhnNumbers_PassesValidation(string cardNumber)
    {
        var card = new Card
        {
            Number = cardNumber,
            ExpireMonth = 12,
            ExpireYear = DateTime.UtcNow.Year + 1,
            Cvv = "123",
            Holder = "JOHN DOE"
        };

        var errors = card.Validate();

        errors.Should().NotContain(e => e.Contains("Luhn"));
    }

    [Theory]
    [InlineData("4532015112830367")]
    [InlineData("1234567890123456")]
    [InlineData("1111111111111112")]
    public void Validate_WithInvalidLuhnNumbers_ReturnsError(string cardNumber)
    {
        var card = new Card
        {
            Number = cardNumber,
            ExpireMonth = 12,
            ExpireYear = DateTime.UtcNow.Year + 1,
            Cvv = "123",
            Holder = "JOHN DOE"
        };

        var errors = card.Validate();

        errors.Should().Contain(e => e.Contains("Luhn"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WithEmptyCardNumber_ReturnsError(string? cardNumber)
    {
        var card = new Card
        {
            Number = cardNumber!,
            ExpireMonth = 12,
            ExpireYear = DateTime.UtcNow.Year + 1,
            Cvv = "123",
            Holder = "JOHN DOE"
        };

        var errors = card.Validate();

        errors.Should().Contain("Card number is required.");
    }

    [Theory]
    [InlineData("123456789012")]
    [InlineData("12345678901234567890")]
    public void Validate_WithWrongLengthCardNumber_ReturnsError(string cardNumber)
    {
        var card = new Card
        {
            Number = cardNumber,
            ExpireMonth = 12,
            ExpireYear = DateTime.UtcNow.Year + 1,
            Cvv = "123",
            Holder = "JOHN DOE"
        };

        var errors = card.Validate();

        errors.Should().Contain(e => e.Contains("between 13 and 19 digits"));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("1234")]
    public void Validate_WithValidCvv_PassesValidation(string cvv)
    {
        var card = new Card
        {
            Number = "4532015112830366",
            ExpireMonth = 12,
            ExpireYear = DateTime.UtcNow.Year + 1,
            Cvv = cvv,
            Holder = "JOHN DOE"
        };

        var errors = card.Validate();

        errors.Should().NotContain(e => e.Contains("CVV"));
    }

    [Theory]
    [InlineData("12")]
    [InlineData("12345")]
    [InlineData("abc")]
    [InlineData("12a")]
    public void Validate_WithInvalidCvv_ReturnsError(string cvv)
    {
        var card = new Card
        {
            Number = "4532015112830366",
            ExpireMonth = 12,
            ExpireYear = DateTime.UtcNow.Year + 1,
            Cvv = cvv,
            Holder = "JOHN DOE"
        };

        var errors = card.Validate();

        errors.Should().Contain("CVV must be 3 or 4 digits.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    [InlineData(-1)]
    public void Validate_WithInvalidExpireMonth_ReturnsError(int month)
    {
        var card = new Card
        {
            Number = "4532015112830366",
            ExpireMonth = month,
            ExpireYear = DateTime.UtcNow.Year + 1,
            Cvv = "123",
            Holder = "JOHN DOE"
        };

        var errors = card.Validate();

        errors.Should().Contain("Expiration month must be between 1 and 12.");
    }

    [Fact]
    public void Validate_WithExpiredCard_ReturnsError()
    {
        var card = new Card
        {
            Number = "4532015112830366",
            ExpireMonth = 1,
            ExpireYear = DateTime.UtcNow.Year - 1,
            Cvv = "123",
            Holder = "JOHN DOE"
        };

        var errors = card.Validate();

        errors.Should().Contain(e => e.Contains("Expiration year"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WithEmptyHolder_ReturnsError(string? holder)
    {
        var card = new Card
        {
            Number = "4532015112830366",
            ExpireMonth = 12,
            ExpireYear = DateTime.UtcNow.Year + 1,
            Cvv = "123",
            Holder = holder!
        };

        var errors = card.Validate();

        errors.Should().Contain("Cardholder name is required.");
    }

    [Fact]
    public void ValidateAndThrow_WithInvalidCard_ThrowsValidationException()
    {
        var card = new Card
        {
            Number = "invalid",
            ExpireMonth = 13,
            ExpireYear = 2020,
            Cvv = "12",
            Holder = ""
        };

        var act = () => card.ValidateAndThrow();

        act.Should().Throw<ValidationException>()
            .Which.Errors.Should().HaveCountGreaterThan(1);
    }

    [Fact]
    public void ValidateAndThrow_WithValidCard_DoesNotThrow()
    {
        var card = new Card
        {
            Number = "4532015112830366",
            ExpireMonth = 12,
            ExpireYear = DateTime.UtcNow.Year + 1,
            Cvv = "123",
            Holder = "JOHN DOE"
        };

        var act = () => card.ValidateAndThrow();

        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_WithFormattedCardNumber_PassesValidation()
    {
        var card = new Card
        {
            Number = "4532 0151 1283 0366",
            ExpireMonth = 12,
            ExpireYear = DateTime.UtcNow.Year + 1,
            Cvv = "123",
            Holder = "JOHN DOE"
        };

        var errors = card.Validate();

        errors.Should().BeEmpty();
    }
}
