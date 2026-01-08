using WayForPaySDK.Responses;

namespace WayForPaySDK.Extensions;

/// <summary>
/// Extension methods for ChargeResponse to simplify 3D Secure detection and handling.
/// </summary>
public static class ChargeResponseExtensions
{
    /// <summary>
    /// Determines whether 3D Secure authentication is required for this charge.
    /// </summary>
    /// <param name="response">The charge response.</param>
    /// <returns>True if 3DS authentication is required; otherwise, false.</returns>
    public static bool Requires3DS(this ChargeResponse response)
    {
        return !string.IsNullOrEmpty(response.ThreeDsAcsUrl);
    }

    /// <summary>
    /// Gets the 3D Secure redirect URL if authentication is required.
    /// </summary>
    /// <param name="response">The charge response.</param>
    /// <returns>The ACS URL for 3DS authentication, or null if not required.</returns>
    public static string? Get3DSRedirectUrl(this ChargeResponse response)
    {
        return response.ThreeDsAcsUrl;
    }

    /// <summary>
    /// Gets the 3D Secure MD parameter needed for authentication.
    /// </summary>
    /// <param name="response">The charge response.</param>
    /// <returns>The MD parameter, or null if 3DS is not required.</returns>
    public static string? Get3DSMd(this ChargeResponse response)
    {
        return response.ThreeDsMd;
    }

    /// <summary>
    /// Gets the 3D Secure PaReq parameter needed for authentication.
    /// </summary>
    /// <param name="response">The charge response.</param>
    /// <returns>The PaReq parameter, or null if 3DS is not required.</returns>
    public static string? Get3DSPaReq(this ChargeResponse response)
    {
        return response.ThreeDsPaReq;
    }

    /// <summary>
    /// Determines whether the charge was approved without 3DS.
    /// </summary>
    /// <param name="response">The charge response.</param>
    /// <returns>True if approved without 3DS; otherwise, false.</returns>
    public static bool IsApprovedWithout3DS(this ChargeResponse response)
    {
        return response.IsSuccess && !response.Requires3DS();
    }
}
