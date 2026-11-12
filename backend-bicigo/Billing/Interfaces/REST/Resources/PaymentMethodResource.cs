using backend_bicigo.Billing.Domain.Model.Aggregates;

namespace backend_bicigo.Billing.Interfaces.REST.Resources;

/// <summary>
///     Response payload for PaymentMethod (matches db.json/paymentMethods).
/// </summary>
public sealed class PaymentMethodResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>User id.</summary>
    public int UserId { get; set; }

    /// <summary>Method type CARD.</summary>
    public string Type { get; set; } = "CARD";

    /// <summary>Masked token.</summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>Whether it is the default method.</summary>
    public bool IsDefault { get; set; }
}
