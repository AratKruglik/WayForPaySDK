using WayForPaySDK.Domain;

namespace WayForPaySDK.Tests.TestData;

/// <summary>
/// Test card data based on WayForPay documentation.
/// </summary>
public static class TestCards
{
    /// <summary>
    /// Visa card that will be approved.
    /// </summary>
    public static Card ApprovedVisa => new()
    {
        Number = "4111111111111111",
        ExpireMonth = 12,
        ExpireYear = 2025,
        Cvv = "123",
        Holder = "TEST CARD"
    };

    /// <summary>
    /// Visa card that will be declined (insufficient funds).
    /// </summary>
    public static Card DeclinedVisa => new()
    {
        Number = "4111111111111112",
        ExpireMonth = 12,
        ExpireYear = 2025,
        Cvv = "123",
        Holder = "TEST CARD"
    };

    /// <summary>
    /// Visa card that requires 3D Secure.
    /// </summary>
    public static Card ThreeDSVisa => new()
    {
        Number = "4111111111111113",
        ExpireMonth = 12,
        ExpireYear = 2025,
        Cvv = "123",
        Holder = "TEST CARD"
    };

    /// <summary>
    /// MasterCard that will be approved.
    /// </summary>
    public static Card ApprovedMasterCard => new()
    {
        Number = "5555555555554444",
        ExpireMonth = 12,
        ExpireYear = 2025,
        Cvv = "123",
        Holder = "TEST CARD"
    };
}
