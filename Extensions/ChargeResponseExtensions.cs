using WayForPaySDK.Responses;

namespace WayForPaySDK.Extensions;

public static class ChargeResponseExtensions
{
    public static bool Requires3DS(this ChargeResponse response)
    {
        return !string.IsNullOrEmpty(response.ThreeDsAcsUrl);
    }

    public static bool IsApprovedWithout3DS(this ChargeResponse response)
    {
        return response.IsSuccess && !response.Requires3DS();
    }
}
