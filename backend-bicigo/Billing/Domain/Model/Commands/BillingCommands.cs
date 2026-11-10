namespace backend_bicigo.Billing.Domain.Model.Commands;

/// <summary>Command to create a subscription (POST /subscriptions).</summary>
public record CreateSubscriptionCommand(
    int UserId,
    int PlanId,
    string PlanType,
    decimal Price,
    string? StartDate = null,
    string? EndDate = null);

/// <summary>Command to create a payment (POST /payments).</summary>
public record CreatePaymentCommand(
    int? SubscriptionId,
    string? TripId,
    int? UserId,
    string? Type,
    decimal Amount,
    string? Currency = null,
    bool IsAutomatic = false,
    string? CreatedAt = null);

/// <summary>Command to create a payment method (POST /payment-methods).</summary>
public record CreatePaymentMethodCommand(
    int UserId,
    string Token,
    string? Type = null,
    bool IsDefault = false);
