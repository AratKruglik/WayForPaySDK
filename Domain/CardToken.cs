namespace WayForPaySDK.Domain;

public sealed record CardToken
{
    public required string Token { get; init; }
}
