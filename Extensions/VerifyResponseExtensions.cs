using WayForPaySDK.Responses;

namespace WayForPaySDK.Extensions;

/// <summary>
/// Extension methods for VerifyResponse to simplify 3D Secure detection and token handling.
/// </summary>
public static class VerifyResponseExtensions
{
    /// <summary>
    /// Determines whether 3D Secure authentication is required for this verification.
    /// </summary>
    /// <param name="response">The verify response.</param>
    /// <returns>True if 3DS authentication is required; otherwise, false.</returns>
    public static bool Requires3DS(this VerifyResponse response)
    {
        return !string.IsNullOrEmpty(response.Url);
    }

    /// <summary>
    /// Gets the 3D Secure redirect URL if authentication is required.
    /// </summary>
    /// <param name="response">The verify response.</param>
    /// <returns>The URL for 3DS authentication, or null if not required.</returns>
    public static string? Get3DSRedirectUrl(this VerifyResponse response)
    {
        return response.Url;
    }

    /// <summary>
    /// Determines whether the verification was successful and a token was created.
    /// </summary>
    /// <param name="response">The verify response.</param>
    /// <returns>True if verification succeeded and recToken is available; otherwise, false.</returns>
    public static bool HasRecToken(this VerifyResponse response)
    {
        return response.IsSuccess && !string.IsNullOrEmpty(response.RecToken);
    }

    /// <summary>
    /// Determines whether the verification was approved without 3DS.
    /// </summary>
    /// <param name="response">The verify response.</param>
    /// <returns>True if approved without 3DS and token is available; otherwise, false.</returns>
    public static bool IsApprovedWithout3DS(this VerifyResponse response)
    {
        return response.IsSuccess && !response.Requires3DS() && response.HasRecToken();
    }
}
