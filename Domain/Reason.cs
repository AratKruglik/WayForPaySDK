using WayForPaySDK.Constants;

namespace WayForPaySDK.Domain;

public sealed record Reason
{
    public required int Code { get; init; }
    public required string Message { get; init; }

    public bool IsSuccess => ReasonCodes.IsSuccess(Code);
    public bool IsWaiting3Ds => ReasonCodes.IsWaiting3Ds(Code);
}
