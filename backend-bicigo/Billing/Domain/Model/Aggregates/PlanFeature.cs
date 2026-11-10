using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Billing.Domain.Model.Aggregates;

/// <summary>
///     Plan feature (normalized from Plan.features string array).
/// </summary>
public class PlanFeature : IAuditableEntity
{
    /// <summary>Protected constructor for EF Core.</summary>
    protected PlanFeature()
    {
        Name = null!;
    }

    /// <summary>Creates a feature from a name.</summary>
    /// <param name="planId">Plan id.</param>
    /// <param name="name">Feature name.</param>
    /// <exception cref="ArgumentException">Thrown when name is empty.</exception>
    public PlanFeature(int planId, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Feature name is required.", nameof(name));
        PlanId = planId;
        Name = name.Trim();
    }

    /// <summary>Server id.</summary>
    public int Id { get; private set; }

    /// <summary>Plan id.</summary>
    public int PlanId { get; private set; }

    /// <summary>Feature name.</summary>
    public string Name { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}
