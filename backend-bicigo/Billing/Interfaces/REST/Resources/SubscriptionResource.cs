using System.Text.Json.Serialization;

namespace backend_bicigo.Billing.Interfaces.REST.Resources;

/// <summary>
///     Response payload for Subscription (matches db.json/subscriptions).
/// </summary>
public sealed class SubscriptionResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>User id.</summary>
    public int UserId { get; set; }

    /// <summary>Plan id.</summary>
    public int PlanId { get; set; }

    /// <summary>PAY_PER_USE | MONTHLY.</summary>
    public string PlanType { get; set; } = string.Empty;

    /// <summary>Price.</summary>
    public decimal Price { get; set; }

    /// <summary>Start date ISO string.</summary>
    public string StartDate { get; set; } = string.Empty;

    /// <summary>End date ISO (nullable).</summary>
    public string? EndDate { get; set; }

    /// <summary>Status ACTIVE.</summary>
    public string Status { get; set; } = "ACTIVE";
}
