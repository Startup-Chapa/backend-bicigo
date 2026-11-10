using System.ComponentModel.DataAnnotations;

namespace backend_bicigo.Billing.Interfaces.REST.Resources;

/// <summary>Create payment-method payload (POST /payment-methods).</summary>
public sealed class CreatePaymentMethodResource
{
    /// <summary>User id.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "InvalidUserId")]
    public int UserId { get; set; }

    /// <summary>Masked token, e.g. ****2777.</summary>
    [Required]
    public string Token { get; set; } = string.Empty;

    /// <summary>Method type CARD.</summary>
    public string? Type { get; set; }

    /// <summary>Whether it is the default method.</summary>
    public bool IsDefault { get; set; }
}
