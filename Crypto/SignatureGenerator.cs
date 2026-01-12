using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using WayForPaySDK.Options;

namespace WayForPaySDK.Crypto;

public sealed class SignatureGenerator : ISignatureGenerator
{
    private const char Delimiter = ';';
    private readonly WayForPayOptions _options;

    public SignatureGenerator(IOptions<WayForPayOptions> options)
    {
        _options = options.Value;
    }

    public string GenerateSignature(IEnumerable<string> fields)
    {
        return GenerateSignature(fields, _options.MerchantSecretKey);
    }

    public string GenerateSignature(IEnumerable<string> fields, string secretKey)
    {
        ArgumentNullException.ThrowIfNull(fields);
        ArgumentException.ThrowIfNullOrEmpty(secretKey);

        var data = string.Join(Delimiter, fields);
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var dataBytes = Encoding.UTF8.GetBytes(data);

#pragma warning disable CA5351 // WayForPay API requires HMACMD5
        using var hmac = new HMACMD5(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);
#pragma warning restore CA5351

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

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
