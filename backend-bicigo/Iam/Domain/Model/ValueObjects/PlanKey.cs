namespace backend_bicigo.Iam.Domain.Model.ValueObjects;

/// <summary>
///     Plan key value object (tarifa | pro).
/// </summary>
public sealed record PlanKey
{
    public const string Tarifa = "tarifa";
    public const string Pro = "pro";

    /// <summary>Initializes a <see cref="PlanKey"/>.</summary>
    /// <param name="value">tarifa or pro (case-insensitive).</param>
    /// <exception cref="ArgumentException">Thrown for unsupported values.</exception>
    public PlanKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Plan cannot be null or empty.", nameof(value));
        var normalized = value.Trim().ToLowerInvariant();
        if (normalized is not (Tarifa or Pro))
            throw new ArgumentException($"Unsupported plan '{value}'. Expected 'tarifa' or 'pro'.", nameof(value));
        Value = normalized;
    }

    /// <summary>Normalized plan key.</summary>
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;
}