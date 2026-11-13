namespace backend_bicigo.Trip.Domain.Model.ValueObjects;

/// <summary>
///     QR validation result value object. Valid: PENDING | VALID | INVALID.
/// </summary>
public sealed record QrValidationResult
{
    /// <summary>Pending.</summary>
    public const string Pending = "PENDING";
    /// <summary>Valid.</summary>
    public const string Valid = "VALID";
    /// <summary>Invalid.</summary>
    public const string Invalid = "INVALID";

    /// <summary>Initializes a <see cref="QrValidationResult"/>.</summary>
    /// <param name="value">Result string.</param>
    /// <exception cref="ArgumentException">Thrown for unsupported values.</exception>
    public QrValidationResult(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("QR result cannot be null or empty.", nameof(value));
        var normalized = value.Trim().ToUpperInvariant();
        if (normalized is not (Pending or Valid or Invalid))
            throw new ArgumentException($"Unsupported QR result '{value}'.", nameof(value));
        Value = normalized;
    }

    /// <summary>Normalized result.</summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;
}
