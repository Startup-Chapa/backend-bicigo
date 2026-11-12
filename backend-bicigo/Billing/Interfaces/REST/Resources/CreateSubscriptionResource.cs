using System.ComponentModel.DataAnnotations;
using backend_bicigo.Billing.Domain.Model.Commands;

namespace backend_bicigo.Billing.Interfaces.REST.Resources;

/// <summary>Create subscription payload (POST /subscriptions).</summary>
public sealed class CreateSubscriptionResource
{
    /// <summary>User id.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "InvalidUserId")]
    public int UserId { get; set; }

    /// <summary>Plan id.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "InvalidPlanId")]
    public int PlanId { get; set; }

    /// <summary>PAY_PER_USE | MONTHLY.</summary>
    [Required]
    public string PlanType { get; set; } = string.Empty;

    /// <summary>Price.</summary>
    [Range(0, 9999999)]
    public decimal Price { get; set; }

    /// <summary>Start date ISO (optional).</summary>
    public string? StartDate { get; set; }

    /// <summary>End date ISO (optional).</summary>
    public string? EndDate { get; set; }
}
