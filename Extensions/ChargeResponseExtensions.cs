using WayForPaySDK.Responses;

namespace WayForPaySDK.Extensions;

public static class ChargeResponseExtensions
{
    public static bool Requires3DS(this ChargeResponse response)
    {
        return !string.IsNullOrEmpty(response.ThreeDsAcsUrl);
    }

    public static string? Get3DSRedirectUrl(this ChargeResponse response)
    {
        return response.ThreeDsAcsUrl;
    }

    public static string? Get3DSMd(this ChargeResponse response)
    {
        return response.ThreeDsMd;
    }

    public static string? Get3DSPaReq(this ChargeResponse response)
    {
        return response.ThreeDsPaReq;
    }

    public static bool IsApprovedWithout3DS(this ChargeResponse response)
    {
        return response.IsSuccess && !response.Requires3DS();
    }
}
