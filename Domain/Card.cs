namespace WayForPaySDK.Domain;

public sealed record Card
{
    public required string Number { get; init; }
    public required int ExpireMonth { get; init; }
    public required int ExpireYear { get; init; }
    public required string Cvv { get; init; }
    public required string Holder { get; init; }
}
