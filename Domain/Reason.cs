using WayForPaySDK.Constants;

namespace WayForPaySDK.Domain;

/// <summary>
/// Represents a reason code and message from the WayForPay API response.
/// </summary>
public sealed record Reason
{
    /// <summary>
    /// Gets the reason code.
    /// </summary>
    public required int Code { get; init; }

    /// <summary>
    /// Gets the reason message.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Gets a value indicating whether the transaction was successful.
    /// </summary>
    public bool IsSuccess => ReasonCodes.IsSuccess(Code);

    /// <summary>
    /// Gets a value indicating whether the transaction is waiting for 3D Secure data.
    /// </summary>
    public bool IsWaiting3Ds => ReasonCodes.IsWaiting3Ds(Code);
}
