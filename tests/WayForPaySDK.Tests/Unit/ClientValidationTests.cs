using FluentAssertions;
using WayForPaySDK.Domain;
using WayForPaySDK.Exceptions;

namespace WayForPaySDK.Tests.Unit;

public class ClientValidationTests
{
    [Fact]
    public void Validate_WithEmptyClient_ReturnsEmptyList()
    {
        var client = new Client();

        var errors = client.Validate();

        errors.Should().BeEmpty();
        client.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithValidClient_ReturnsEmptyList()
    {
        var client = new Client
        {
            Id = "12345",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Phone = "+380501234567",
            Country = "UA",
            IpAddress = "192.168.1.1"
        };

        var errors = client.Validate();

        errors.Should().BeEmpty();
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("user+tag@gmail.com")]
    [InlineData("a@b.co")]
    public void Validate_WithValidEmails_PassesValidation(string email)
    {
        var client = new Client { Email = email };

        var errors = client.Validate();

        errors.Should().NotContain(e => e.Contains("email"));
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user@domain")]
    [InlineData("user name@example.com")]
    public void Validate_WithInvalidEmails_ReturnsError(string email)
    {
        var client = new Client { Email = email };

        var errors = client.Validate();

        errors.Should().Contain("Invalid email format.");
    }

    [Theory]
    [InlineData("+380501234567")]
    [InlineData("(050) 123-45-67")]
    [InlineData("050-123-45-67")]
    [InlineData("0501234567")]
    public void Validate_WithValidPhones_PassesValidation(string phone)
    {
        var client = new Client { Phone = phone };

        var errors = client.Validate();

        errors.Should().NotContain(e => e.Contains("phone"));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("abc")]
    [InlineData("test@phone")]
    [InlineData("123456789012345678901")]
    public void Validate_WithInvalidPhones_ReturnsError(string phone)
    {
        var client = new Client { Phone = phone };

        var errors = client.Validate();

        errors.Should().Contain(e => e.Contains("phone"));
    }

    [Theory]
    [InlineData("192.168.1.1")]
    [InlineData("10.0.0.1")]
    [InlineData("::1")]
    [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334")]
    public void Validate_WithValidIpAddresses_PassesValidation(string ip)
    {
        var client = new Client { IpAddress = ip };

        var errors = client.Validate();

        errors.Should().NotContain(e => e.Contains("IP"));
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("256.256.256.256")]
    [InlineData("abc.def.ghi.jkl")]
    [InlineData("192.168.1.1.1")]
    public void Validate_WithInvalidIpAddresses_ReturnsError(string ip)
    {
        var client = new Client { IpAddress = ip };

        var errors = client.Validate();

        errors.Should().Contain("Invalid IP address format.");
    }

    [Fact]
    public void Validate_WithTooLongCountry_ReturnsError()
    {
        var client = new Client { Country = new string('A', 101) };

        var errors = client.Validate();

        errors.Should().Contain("Country name is too long (max 100 characters).");
    }

    [Fact]
    public void Validate_WithTooLongFirstName_ReturnsError()
    {
        var client = new Client { FirstName = new string('A', 101) };

        var errors = client.Validate();

        errors.Should().Contain("First name is too long (max 100 characters).");
    }

    [Fact]
    public void Validate_WithTooLongLastName_ReturnsError()
    {
        var client = new Client { LastName = new string('A', 101) };

        var errors = client.Validate();

        errors.Should().Contain("Last name is too long (max 100 characters).");
    }

    [Fact]
    public void ValidateAndThrow_WithInvalidClient_ThrowsValidationException()
    {
        var client = new Client
        {
            Email = "invalid",
            Phone = "12",
            IpAddress = "invalid"
        };

        var act = () => client.ValidateAndThrow();

        act.Should().Throw<ValidationException>()
            .Which.Errors.Should().HaveCount(3);
    }

    [Fact]
    public void ValidateAndThrow_WithValidClient_DoesNotThrow()
    {
        var client = new Client
        {
            Email = "test@example.com",
            Phone = "+380501234567",
            IpAddress = "192.168.1.1"
        };

        var act = () => client.ValidateAndThrow();

        act.Should().NotThrow();
    }
}
