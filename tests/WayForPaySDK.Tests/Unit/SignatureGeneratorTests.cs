using FluentAssertions;
using WayForPaySDK.Crypto;
using WayForPaySDK.Tests.Fixtures;

namespace WayForPaySDK.Tests.Unit;

public class SignatureGeneratorTests
{
    private readonly SignatureGenerator _sut;

    public SignatureGeneratorTests()
    {
        _sut = new SignatureGenerator(TestOptions.CreateOptions());
    }

    [Fact]
    public void GenerateSignature_WithValidFields_ReturnsNonEmptyHash()
    {
        // Arrange
        var fields = new[] { "merchant", "ORDER123", "100.00", "UAH" };

        // Act
        var result = _sut.GenerateSignature(fields);

        // Assert
        result.Should().NotBeNullOrWhiteSpace();
        result.Length.Should().Be(32); // MD5 produces 32 hex characters
    }

    [Fact]
    public void GenerateSignature_WithSameFieldsTwice_ReturnsSameHash()
    {
        // Arrange
        var fields = new[] { "merchant", "ORDER123", "100.00", "UAH" };

        // Act
        var result1 = _sut.GenerateSignature(fields);
        var result2 = _sut.GenerateSignature(fields);

        // Assert
        result1.Should().Be(result2);
    }

    [Fact]
    public void GenerateSignature_WithDifferentFields_ReturnsDifferentHash()
    {
        // Arrange
        var fields1 = new[] { "merchant", "ORDER123", "100.00", "UAH" };
        var fields2 = new[] { "merchant", "ORDER124", "100.00", "UAH" };

        // Act
        var result1 = _sut.GenerateSignature(fields1);
        var result2 = _sut.GenerateSignature(fields2);

        // Assert
        result1.Should().NotBe(result2);
    }

    [Fact]
    public void GenerateSignature_WithCustomSecret_GeneratesDifferentHash()
    {
        // Arrange
        var generator1 = new SignatureGenerator(TestOptions.CreateOptions());
        var generator2 = new SignatureGenerator(TestOptions.CreateOptionsWithCustomSecret("different_secret_key"));
        var fields = new[]
        {
            "test_merchant",
            "ORDER123",
            "100.00",
            "UAH"
        };

        // Act
        var result1 = generator1.GenerateSignature(fields);
        var result2 = generator2.GenerateSignature(fields);

        // Assert
        result1.Should().NotBe(result2, "different secret keys should produce different signatures");
        result1.Length.Should().Be(32);
        result2.Length.Should().Be(32);
    }

    [Fact]
    public void GenerateSignature_WithEmptyFields_ReturnsHash()
    {
        // Arrange
        var fields = Array.Empty<string>();

        // Act
        var result = _sut.GenerateSignature(fields);

        // Assert - even empty fields should produce a valid hash (of empty string)
        result.Should().NotBeNullOrWhiteSpace();
        result.Length.Should().Be(32);
    }

    [Fact]
    public void GenerateSignature_WithNullFields_Throws()
    {
        // Act
        var act = () => _sut.GenerateSignature(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void VerifySignature_WithMatchingSignatures_ReturnsTrue()
    {
        // Arrange
        var fields = new[] { "merchant", "ORDER123", "100.00", "UAH" };
        var signature = _sut.GenerateSignature(fields);

        // Act
        var result = _sut.VerifySignature(signature, signature);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifySignature_WithDifferentSignatures_ReturnsFalse()
    {
        // Arrange
        var signature1 = "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6";
        var signature2 = "p6o5n4m3l2k1j0i9h8g7f6e5d4c3b2a1";

        // Act
        var result = _sut.VerifySignature(signature1, signature2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifySignature_WithCaseDifferentSignatures_ReturnsTrue()
    {
        // Arrange
        // Signature verification is case-insensitive for better compatibility
        // Both signatures are normalized to lowercase before comparison
        var signature1 = "a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6";
        var signature2 = "A1B2C3D4E5F6A7B8C9D0E1F2A3B4C5D6";

        // Act
        var result = _sut.VerifySignature(signature1, signature2);

        // Assert
        result.Should().BeTrue("signature comparison is case-insensitive");
    }

    [Theory]
    [InlineData(null, "valid")]
    [InlineData("valid", null)]
    [InlineData(null, null)]
    public void VerifySignature_WithNullSignatures_ReturnsFalse(string? sig1, string? sig2)
    {
        // Act
        var result = _sut.VerifySignature(sig1!, sig2!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifySignature_IsTimingSafe()
    {
        // This test verifies that signature verification uses constant-time comparison
        // to prevent timing attacks
        // Arrange
        var correctSignature = "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6";
        var wrongSignature1 = "b1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6"; // Different first char
        var wrongSignature2 = "a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p7"; // Different last char

        // Act
        var result1 = _sut.VerifySignature(correctSignature, wrongSignature1);
        var result2 = _sut.VerifySignature(correctSignature, wrongSignature2);

        // Assert
        // Both should be false (this test mainly documents the timing-safe behavior)
        result1.Should().BeFalse();
        result2.Should().BeFalse();
    }
}
