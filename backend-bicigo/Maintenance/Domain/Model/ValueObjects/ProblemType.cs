namespace backend_bicigo.Maintenance.Domain.Model.ValueObjects;

public sealed record ProblemType
{
    /// <summary>Brakes.</summary>
    public const string Brakes = "BRAKES";
    /// <summary>Chain.</summary>
    public const string Chain = "CHAIN";
    /// <summary>Wheel.</summary>
    public const string Wheel = "WHEEL";
    /// <summary>Seat.</summary>
    public const string Seat = "SEAT";
    /// <summary>Lights.</summary>
    public const string Lights = "LIGHTS";
    /// <summary>Other.</summary>
    public const string Other = "OTHER";
    
    public ProblemType(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Problem type cannot be null or empty.", nameof(value));
        var normalized = value.Trim().ToUpperInvariant();
        if (normalized is not (Brakes or Chain or Wheel or Seat or Lights or Other))
            throw new ArgumentException($"Unsupported problem type '{value}'.", nameof(value));
        Value = normalized;
    }
    
    public string Value { get; }

    /// <inheritdoc />
    public override string ToString() => Value;
}