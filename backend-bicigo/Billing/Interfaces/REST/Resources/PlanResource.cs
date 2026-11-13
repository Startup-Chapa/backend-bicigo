using backend_bicigo.Billing.Domain.Model.Aggregates;

namespace backend_bicigo.Billing.Interfaces.REST.Resources;

/// <summary>
///     Response payload for Plan (matches db.json/plans).
/// </summary>
public sealed class PlanResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>Display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>PAY_PER_USE | MONTHLY.</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Fixed price.</summary>
    public decimal Price { get; set; }

    /// <summary>Price per km.</summary>
    public decimal PricePerKm { get; set; }

    /// <summary>Currency PEN.</summary>
    public string Currency { get; set; } = "PEN";

    /// <summary>Description.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Features as array (DB stores comma-separated).</summary>
    public string[] Features { get; set; } = [];

    /// <summary>Whether it is recommended.</summary>
    public bool Recommended { get; set; }

    /// <summary>Whether it is the current plan.</summary>
    public bool Current { get; set; }
}
