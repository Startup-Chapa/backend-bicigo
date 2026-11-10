namespace backend_bicigo.Billing.Domain.Model.ValueObjects;

/// <summary>
///     Payment type value object. Valid values: CHARGE | TOPUP (or null for legacy rows).
/// </summary>
public sealed record PaymentType
{
    /// <summary>Trip charge.</summary>
    public const string Charge = "CHARGE";

    /// <summary>Balance top-up.</summary>
    public const string TopUp = "TOPUP";

    /// <summary>Initializes a <see cref="PaymentType"/>.</summary>
    /// <param name="value">CHARGE or TOPUP.</param>
    /// <exception cref="ArgumentException">Thrown for unsupported values.</exception>
    public PaymentType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Payment type cannot be null or empty.", nameof(value));
        var normalized = value.Trim().ToUpperInvariant();
        if (normalized is not (Charge or TopUp))
            throw new ArgumentException($"Unsupported payment type '{value}'.", nameof(value));
        Value = normalized;
    }

    /// <summary>Normalized payment type.</summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;
}
