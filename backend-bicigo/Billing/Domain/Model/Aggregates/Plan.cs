using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Billing.Domain.Model.Aggregates;

/// <summary>
///     Plan aggregate. Mirrors <c>billing/domain/model/plan.js</c> and <c>db.json/plans</c>.
/// </summary>
public class Plan : IAuditableEntity
{
    /// <summary>Protected parameterless constructor for EF Core.</summary>
    protected Plan()
    {
        Name = null!;
        Type = null!;
        Description = string.Empty;
        Features = string.Empty;
        Currency = "PEN";
    }

    /// <summary>Server id.</summary>
    public int Id { get; private set; }

    /// <summary>Display name, e.g. "Plan Tarifa".</summary>
    public string Name { get; private set; }

    /// <summary>Type: PAY_PER_USE | MONTHLY.</summary>
    public string Type { get; private set; }

    /// <summary>Fixed price.</summary>
    public decimal Price { get; private set; }

    /// <summary>Price per km in PEN (0.29 for tariff).</summary>
    public decimal PricePerKm { get; private set; }

    /// <summary>Currency, always PEN.</summary>
    public string Currency { get; private set; }

    /// <summary>Description.</summary>
    public string Description { get; private set; }

    /// <summary>Comma-separated features (stored as text; JSON array is mapped at the API layer).</summary>
    public string Features { get; private set; }

    /// <summary>Whether it is the recommended plan.</summary>
    public bool Recommended { get; private set; }

    /// <summary>Whether it is the current plan for the requesting user (ctx-specific).</summary>
    public bool Current { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}
