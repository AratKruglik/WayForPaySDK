using WayForPaySDK.Domain.Enums;

namespace WayForPaySDK.Domain;

/// <summary>
/// Represents regular (recurring) payment configuration.
/// </summary>
public sealed record Regular
{
    /// <summary>
    /// Gets the allowed payment frequency modes.
    /// </summary>
    public required IReadOnlyList<RegularMode> Modes { get; init; }

    /// <summary>
    /// Gets the regular payment amount.
    /// </summary>
    public decimal? Amount { get; init; }

    /// <summary>
    /// Gets the next payment date.
    /// </summary>
    public DateOnly? DateNext { get; init; }

    /// <summary>
    /// Gets the end date for regular payments.
    /// </summary>
    public DateOnly? DateEnd { get; init; }

    /// <summary>
    /// Gets the total number of regular payments.
    /// </summary>
    public int? Count { get; init; }

    /// <summary>
    /// Gets a value indicating whether regular payments are enabled.
    /// </summary>
    public bool? IsOn { get; init; }

    /// <summary>
    /// Gets the regular payment behavior.
    /// </summary>
    public RegularBehavior? Behavior { get; init; }
}
