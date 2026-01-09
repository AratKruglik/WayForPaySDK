namespace WayForPaySDK.Domain;

public sealed record Product
{
    public required string Name { get; init; }
    public required decimal Price { get; init; }
    public required int Count { get; init; }
}
