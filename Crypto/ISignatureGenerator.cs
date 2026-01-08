namespace WayForPaySDK.Crypto;

/// <summary>
/// Interface for signature generation and verification.
/// </summary>
public interface ISignatureGenerator
{
    /// <summary>
    /// Generates a signature from the given fields using the configured secret key.
    /// </summary>
    /// <param name="fields">The fields to include in the signature.</param>
    /// <returns>The generated signature as a lowercase hexadecimal string.</returns>
    string GenerateSignature(IEnumerable<string> fields);

    /// <summary>
    /// Generates a signature from the given fields using a specified secret key.
    /// </summary>
    /// <param name="fields">The fields to include in the signature.</param>
    /// <param name="secretKey">The secret key to use for signing.</param>
    /// <returns>The generated signature as a lowercase hexadecimal string.</returns>
    string GenerateSignature(IEnumerable<string> fields, string secretKey);

    /// <summary>
    /// Verifies that the actual signature matches the expected signature using timing-safe comparison.
    /// </summary>
    /// <param name="expected">The expected signature.</param>
    /// <param name="actual">The actual signature to verify.</param>
    /// <returns>True if the signatures match; otherwise, false.</returns>
    bool VerifySignature(string expected, string actual);
}
