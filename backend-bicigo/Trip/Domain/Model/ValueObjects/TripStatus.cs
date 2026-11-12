namespace backend_bicigo.Trip.Domain.Model.ValueObjects;

/// <summary>
///     Trip status value object. Valid: PENDING | IN_PROGRESS | FINISHED.
/// </summary>
public sealed record TripStatus
{
    /// <summary>Pending.</summary>
    public const string Pending = "PENDING";
    /// <summary>In progress.</summary>
    public const string InProgress = "IN_PROGRESS";
    /// <summary>Finished.</summary>
    public const string Finished = "FINISHED";

    /// <summary>Initializes a <see cref="TripStatus"/>.</summary>
    /// <param name="value">Status string.</param>
    /// <exception cref="ArgumentException">Thrown for unsupported values.</exception>
    public TripStatus(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Trip status cannot be null or empty.", nameof(value));
        var normalized = value.Trim().ToUpperInvariant();
        if (normalized is not (Pending or InProgress or Finished))
            throw new ArgumentException($"Unsupported trip status '{value}'.", nameof(value));
        Value = normalized;
    }

    /// <summary>Normalized status.</summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;
}
