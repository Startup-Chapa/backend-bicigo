using backend_bicigo.Billing.Domain.Model.Aggregates;

namespace backend_bicigo.Billing.Interfaces.REST.Resources;

/// <summary>
///     Response payload for Payment (matches db.json/payments).
/// </summary>
public sealed class PaymentResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>Subscription id (nullable).</summary>
    public int? SubscriptionId { get; set; }

    /// <summary>Trip id (nullable).</summary>
    public string? TripId { get; set; }

    /// <summary>User id (nullable).</summary>
    public int? UserId { get; set; }

    /// <summary>CHARGE | TOPUP | null.</summary>
    public string? Type { get; set; }

    /// <summary>Amount.</summary>
    public decimal Amount { get; set; }

    /// <summary>Currency PEN.</summary>
    public string Currency { get; set; } = "PEN";

    /// <summary>Status COMPLETED.</summary>
    public string Status { get; set; } = "COMPLETED";

    /// <summary>Whether it was automatic.</summary>
    public bool IsAutomatic { get; set; }

    /// <summary>Created timestamp ISO string.</summary>
    public string CreatedAt { get; set; } = string.Empty;
}
