using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Shared.Domain.Repositories;

namespace backend_bicigo.Billing.Domain.Model.ValueObjects;

/// <summary>
///     Plan type value object. Valid values: PAY_PER_USE | MONTHLY.
/// </summary>
public sealed record PlanType
{
    /// <summary>Pay-per-use tariff plan.</summary>
    public const string PayPerUse = "PAY_PER_USE";

    /// <summary>Monthly Pro subscription.</summary>
    public const string Monthly = "MONTHLY";

    /// <summary>Initializes a <see cref="PlanType"/>.</summary>
    /// <param name="value">PAY_PER_USE or MONTHLY.</param>
    /// <exception cref="ArgumentException">Thrown for unsupported values.</exception>
    public PlanType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Plan type cannot be null or empty.", nameof(value));
        var normalized = value.Trim().ToUpperInvariant();
        if (normalized is not (PayPerUse or Monthly))
            throw new ArgumentException($"Unsupported plan type '{value}'.", nameof(value));
        Value = normalized;
    }

    /// <summary>Normalized plan type.</summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;
}
