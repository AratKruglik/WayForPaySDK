using WayForPaySDK.Domain.Enums;

namespace WayForPaySDK.Domain;

public sealed record Regular
{
    public required IReadOnlyList<RegularMode> Modes { get; init; }
    public decimal? Amount { get; init; }
    public DateOnly? DateNext { get; init; }
    public DateOnly? DateEnd { get; init; }
    public int? Count { get; init; }
    public bool? IsOn { get; init; }
    public RegularBehavior? Behavior { get; init; }
}
