using WayForPaySDK.Responses;

namespace WayForPaySDK.Extensions;

public static class VerifyResponseExtensions
{
    public static bool Requires3DS(this VerifyResponse response)
    {
        return !string.IsNullOrEmpty(response.Url);
    }

    public static string? Get3DSRedirectUrl(this VerifyResponse response)
    {
        return response.Url;
    }

    public static bool HasRecToken(this VerifyResponse response)
    {
        return response.IsSuccess && !string.IsNullOrEmpty(response.RecToken);
    }

    public static bool IsApprovedWithout3DS(this VerifyResponse response)
    {
        return response.IsSuccess && !response.Requires3DS() && response.HasRecToken();
    }
}
