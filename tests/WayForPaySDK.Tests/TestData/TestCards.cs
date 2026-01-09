using WayForPaySDK.Domain;

namespace WayForPaySDK.Tests.TestData;

/// Test cards based on WayForPay documentation
public static class TestCards
{
    /// Approved transaction
    public static Card ApprovedVisa => new()
    {
        Number = "4111111111111111",
        ExpireMonth = 12,
        ExpireYear = 2025,
        Cvv = "123",
        Holder = "TEST CARD"
    };

    /// Declined (insufficient funds)
    public static Card DeclinedVisa => new()
    {
        Number = "4111111111111112",
        ExpireMonth = 12,
        ExpireYear = 2025,
        Cvv = "123",
        Holder = "TEST CARD"
    };

    /// Requires 3D Secure
    public static Card ThreeDSVisa => new()
    {
        Number = "4111111111111113",
        ExpireMonth = 12,
        ExpireYear = 2025,
        Cvv = "123",
        Holder = "TEST CARD"
    };

    /// Approved transaction
    public static Card ApprovedMasterCard => new()
    {
        Number = "5555555555554444",
        ExpireMonth = 12,
        ExpireYear = 2025,
        Cvv = "123",
        Holder = "TEST CARD"
    };
}
