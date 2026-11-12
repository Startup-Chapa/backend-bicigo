namespace backend_bicigo.Billing.Interfaces.REST.Resources;

/// <summary>Plan feature response (normalized features).</summary>
public sealed class PlanFeatureResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>Plan id.</summary>
    public int PlanId { get; set; }

    /// <summary>Feature name.</summary>
    public string Name { get; set; } = string.Empty;
}
