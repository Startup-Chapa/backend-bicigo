using backend_bicigo.Billing.Domain.Model.Commands;
using backend_bicigo.Billing.Domain.Model.ValueObjects;
using backend_bicigo.Billing.Interfaces.REST.Resources;

namespace backend_bicigo.Billing.Interfaces.REST.Transform;

/// <summary>
///     Assembles Billing commands from create resources.
/// </summary>
public static class BillingCommandsFromResourcesAssembler
{
    /// <summary>Assembles a subscription command.</summary>
    /// <param name="resource">The create resource.</param>
    /// <returns>The command.</returns>
    public static CreateSubscriptionCommand ToSubscriptionCommand(CreateSubscriptionResource resource) =>
        new(
            UserId: resource.UserId,
            PlanId: resource.PlanId,
            PlanType: new PlanType(resource.PlanType).Value,
            Price: resource.Price,
            StartDate: resource.StartDate,
            EndDate: resource.EndDate);

    /// <summary>Assembles a payment command.</summary>
    /// <param name="resource">The create resource.</param>
    /// <returns>The command.</returns>
    public static CreatePaymentCommand ToPaymentCommand(CreatePaymentResource resource) =>
        new(
            SubscriptionId: resource.SubscriptionId,
            TripId: resource.TripId,
            UserId: resource.UserId,
            Type: string.IsNullOrWhiteSpace(resource.Type) ? null : new PaymentType(resource.Type).Value,
            Amount: resource.Amount,
            Currency: resource.Currency,
            IsAutomatic: resource.IsAutomatic,
            CreatedAt: resource.CreatedAt);

    /// <summary>Assembles a payment-method command.</summary>
    /// <param name="resource">The create resource.</param>
    /// <returns>The command.</returns>
    public static CreatePaymentMethodCommand ToPaymentMethodCommand(CreatePaymentMethodResource resource) =>
        new(
            UserId: resource.UserId,
            Token: resource.Token.Trim(),
            Type: resource.Type,
            IsDefault: resource.IsDefault);
}
