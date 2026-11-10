using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Billing.Domain.Model.Queries;

namespace backend_bicigo.Billing.Application.Services;

/// <summary>Query service contract for Billing reads.</summary>
public interface IBillingQueryService
{
    /// <summary>Handles a get-plan-by-id query.</summary>
    Task<Plan?> Handle(GetPlanByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Handles a subscriptions-by-user query.</summary>
    Task<IEnumerable<Subscription>> Handle(GetSubscriptionsByUserQuery query, CancellationToken cancellationToken = default);

    /// <summary>Handles a payments-by-user query.</summary>
    Task<IEnumerable<Payment>> Handle(GetPaymentsByUserQuery query, CancellationToken cancellationToken = default);

    /// <summary>Handles a payment-methods-by-user query.</summary>
    Task<IEnumerable<PaymentMethod>> Handle(GetPaymentMethodsByUserQuery query, CancellationToken cancellationToken = default);
}
