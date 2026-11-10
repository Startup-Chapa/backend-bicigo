using System.ComponentModel.DataAnnotations;

namespace backend_bicigo.Billing.Interfaces.REST.Resources;

/// <summary>Create payment payload (POST /payments).</summary>
public sealed class CreatePaymentResource
{
    /// <summary>Subscription id (nullable).</summary>
    public int? SubscriptionId { get; set; }

    /// <summary>Trip id (nullable).</summary>
    public string? TripId { get; set; }

    /// <summary>User id (nullable).</summary>
    public int? UserId { get; set; }

    /// <summary>CHARGE | TOPUP | null.</summary>
    public string? Type { get; set; }

    /// <summary>Amount.</summary>
    [Range(0, 9999999)]
    public decimal Amount { get; set; }

    /// <summary>Currency PEN.</summary>
    public string? Currency { get; set; }

    /// <summary>Whether it was automatic.</summary>
    public bool IsAutomatic { get; set; }

    /// <summary>Created timestamp ISO (optional).</summary>
    public string? CreatedAt { get; set; }
}
