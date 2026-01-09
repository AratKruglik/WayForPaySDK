namespace WayForPaySDK.Crypto;

public interface ISignatureGenerator
{
    string GenerateSignature(IEnumerable<string> fields);
    string GenerateSignature(IEnumerable<string> fields, string secretKey);
    bool VerifySignature(string expected, string actual);
}
