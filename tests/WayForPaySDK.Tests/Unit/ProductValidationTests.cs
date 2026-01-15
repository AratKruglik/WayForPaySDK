using FluentAssertions;
using WayForPaySDK.Domain;
using WayForPaySDK.Exceptions;

namespace WayForPaySDK.Tests.Unit;

public class ProductValidationTests
{
    [Fact]
    public void Validate_WithValidProduct_ReturnsEmptyList()
    {
        var product = new Product
        {
            Name = "Test Product",
            Price = 100.00m,
            Count = 2
        };

        var errors = product.Validate();

        errors.Should().BeEmpty();
        product.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WithEmptyName_ReturnsError(string? name)
    {
        var product = new Product
        {
            Name = name!,
            Price = 100.00m,
            Count = 1
        };

        var errors = product.Validate();

        errors.Should().Contain("Product name is required.");
    }

    [Fact]
    public void Validate_WithTooLongName_ReturnsError()
    {
        var product = new Product
        {
            Name = new string('A', 1001),
            Price = 100.00m,
            Count = 1
        };

        var errors = product.Validate();

        errors.Should().Contain("Product name is too long (max 1000 characters).");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Validate_WithInvalidPrice_ReturnsError(decimal price)
    {
        var product = new Product
        {
            Name = "Test Product",
            Price = price,
            Count = 1
        };

        var errors = product.Validate();

        errors.Should().Contain("Product price must be greater than zero.");
    }

    [Theory]
    [InlineData(0.01)]
    [InlineData(1)]
    [InlineData(999999.99)]
    public void Validate_WithValidPrice_PassesValidation(decimal price)
    {
        var product = new Product
        {
            Name = "Test Product",
            Price = price,
            Count = 1
        };

        var errors = product.Validate();

        errors.Should().NotContain(e => e.Contains("price"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WithInvalidCount_ReturnsError(int count)
    {
        var product = new Product
        {
            Name = "Test Product",
            Price = 100.00m,
            Count = count
        };

        var errors = product.Validate();

        errors.Should().Contain("Product count must be greater than zero.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(1000)]
    public void Validate_WithValidCount_PassesValidation(int count)
    {
        var product = new Product
        {
            Name = "Test Product",
            Price = 100.00m,
            Count = count
        };

        var errors = product.Validate();

        errors.Should().NotContain(e => e.Contains("count"));
    }

    [Fact]
    public void ValidateAndThrow_WithInvalidProduct_ThrowsValidationException()
    {
        var product = new Product
        {
            Name = "",
            Price = 0,
            Count = 0
        };

        var act = () => product.ValidateAndThrow();

        act.Should().Throw<ValidationException>()
            .Which.Errors.Should().HaveCount(3);
    }

    [Fact]
    public void ValidateAndThrow_WithValidProduct_DoesNotThrow()
    {
        var product = new Product
        {
            Name = "Test Product",
            Price = 100.00m,
            Count = 1
        };

        var act = () => product.ValidateAndThrow();

        act.Should().NotThrow();
    }

    [Fact]
    public void IsValid_WithValidProduct_ReturnsTrue()
    {
        var product = new Product
        {
            Name = "Test Product",
            Price = 100.00m,
            Count = 1
        };

        product.IsValid.Should().BeTrue();
    }

    [Fact]
    public void IsValid_WithInvalidProduct_ReturnsFalse()
    {
        var product = new Product
        {
            Name = "",
            Price = 0,
            Count = 0
        };

        product.IsValid.Should().BeFalse();
    }
}
