using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using WayForPaySDK.Options;

namespace WayForPaySDK.Crypto;

/// <summary>
/// Generates and verifies HMAC-MD5 signatures for WayForPay API requests.
/// </summary>
public sealed class SignatureGenerator : ISignatureGenerator
{
    private const char Delimiter = ';';
    private readonly WayForPayOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="SignatureGenerator"/> class.
    /// </summary>
    /// <param name="options">The WayForPay options.</param>
    public SignatureGenerator(IOptions<WayForPayOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc />
    public string GenerateSignature(IEnumerable<string> fields)
    {
        return GenerateSignature(fields, _options.MerchantSecretKey);
    }

    /// <inheritdoc />
    public string GenerateSignature(IEnumerable<string> fields, string secretKey)
    {
        ArgumentNullException.ThrowIfNull(fields);
        ArgumentException.ThrowIfNullOrEmpty(secretKey);

        var data = string.Join(Delimiter, fields);
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var dataBytes = Encoding.UTF8.GetBytes(data);

#pragma warning disable CA5351 // WayForPay API requires MD5
        var hashBytes = HMACMD5.HashData(keyBytes, dataBytes);
#pragma warning restore CA5351

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    /// <inheritdoc />
    public bool VerifySignature(string expected, string actual)
    {
        if (string.IsNullOrEmpty(expected) || string.IsNullOrEmpty(actual))
        {
            return false;
        }

        var expectedBytes = Encoding.UTF8.GetBytes(expected.ToLowerInvariant());
        var actualBytes = Encoding.UTF8.GetBytes(actual.ToLowerInvariant());

        return CryptographicOperations.FixedTimeEquals(expectedBytes, actualBytes);
    }
}
