using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Billing.Interfaces.REST.Resources;

namespace backend_bicigo.Billing.Interfaces.REST.Transform;

/// <summary>
///     Assembles Billing resources from entities (Plan, Subscription, Payment, PaymentMethod).
/// </summary>
public static class BillingResourceFromEntityAssembler
{
    /// <summary>Assembles a PlanResource from a Plan.</summary>
    /// <param name="entity">The plan.</param>
    /// <returns>The resource.</returns>
    public static PlanResource ToPlanFromEntity(Plan entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Type = entity.Type,
        Price = entity.Price,
        PricePerKm = entity.PricePerKm,
        Currency = entity.Currency,
        Description = entity.Description,
        Features = string.IsNullOrWhiteSpace(entity.Features)
            ? []
            : entity.Features.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
        Recommended = entity.Recommended,
        Current = entity.Current
    };

    /// <summary>Assembles a SubscriptionResource from a Subscription.</summary>
    /// <param name="entity">The subscription.</param>
    /// <returns>The resource.</returns>
    public static SubscriptionResource ToSubscriptionFromEntity(Subscription entity) => new()
    {
        Id = entity.Id,
        UserId = entity.UserId,
        PlanId = entity.PlanId,
        PlanType = entity.PlanType,
        Price = entity.Price,
        StartDate = entity.StartDate,
        EndDate = entity.EndDate,
        Status = entity.Status
    };

    /// <summary>Assembles a PaymentResource from a Payment.</summary>
    /// <param name="entity">The payment.</param>
    /// <returns>The resource.</returns>
    public static PaymentResource ToPaymentFromEntity(Payment entity) => new()
    {
        Id = entity.Id,
        SubscriptionId = entity.SubscriptionId,
        TripId = entity.TripId,
        UserId = entity.UserId,
        Type = entity.Type,
        Amount = entity.Amount,
        Currency = entity.Currency,
        Status = entity.Status,
        IsAutomatic = entity.IsAutomatic,
        CreatedAt = entity.CreatedAt
    };

    /// <summary>Assembles a PaymentMethodResource from a PaymentMethod.</summary>
    /// <param name="entity">The payment method.</param>
    /// <returns>The resource.</returns>
    public static PaymentMethodResource ToPaymentMethodFromEntity(PaymentMethod entity) => new()
    {
        Id = entity.Id,
        UserId = entity.UserId,
        Type = entity.Type,
        Token = entity.Token,
        IsDefault = entity.IsDefault
    };
}
