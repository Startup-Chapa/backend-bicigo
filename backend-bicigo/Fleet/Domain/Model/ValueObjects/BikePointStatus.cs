namespace backend_bicigo.Fleet.Domain.Model.ValueObjects;

/// <summary>
///     Bike-point status value object. Valid: OPERATIONAL | FULL | MAINTENANCE | DISABLED.
/// </summary>
public sealed record BikePointStatus
{
    /// <summary>Open for docking.</summary>
    public const string Operational = "OPERATIONAL";
    /// <summary>At capacity.</summary>
    public const string Full = "FULL";
    /// <summary>Under maintenance.</summary>
    public const string Maintenance = "MAINTENANCE";
    /// <summary>Disabled.</summary>
    public const string Disabled = "DISABLED";

    /// <summary>Initializes a <see cref="BikePointStatus"/>.</summary>
    /// <param name="value">Status string.</param>
    /// <exception cref="ArgumentException">Thrown for unsupported values.</exception>
    public BikePointStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Bike-point status cannot be null or empty.", nameof(value));
        var normalized = value.Trim().ToUpperInvariant();
        if (normalized is not (Operational or Full or Maintenance or Disabled))
            throw new ArgumentException($"Unsupported bike-point status '{value}'.", nameof(value));
        Value = normalized;
    }

    /// <summary>Normalized status.</summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;
}
