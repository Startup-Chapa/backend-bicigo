using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Shared.Domain.Repositories;

namespace backend_bicigo.Fleet.Domain.Model.ValueObjects;

/// <summary>
///     Bicycle status value object. Valid: AVAILABLE | IN_USE | MAINTENANCE | DISABLED.
/// </summary>
public sealed record BicycleStatus
{
    /// <summary>Available at a station.</summary>
    public const string Available = "AVAILABLE";
    /// <summary>Currently ridden.</summary>
    public const string InUse = "IN_USE";
    /// <summary>Under maintenance.</summary>
    public const string Maintenance = "MAINTENANCE";
    /// <summary>Disabled.</summary>
    public const string Disabled = "DISABLED";

    /// <summary>Initializes a <see cref="BicycleStatus"/>.</summary>
    /// <param name="value">Status string.</param>
    /// <exception cref="ArgumentException">Thrown for unsupported values.</exception>
    public BicycleStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Bicycle status cannot be null or empty.", nameof(value));
        var normalized = value.Trim().ToUpperInvariant();
        if (normalized is not (Available or InUse or Maintenance or Disabled))
            throw new ArgumentException($"Unsupported bicycle status '{value}'.", nameof(value));
        Value = normalized;
    }

    /// <summary>Normalized status.</summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <summary>No-op marker to keep the ValueObjects folder consistent.</summary>
    internal static void EnsureMarker() => _ = typeof(IUnitOfWork);
}
