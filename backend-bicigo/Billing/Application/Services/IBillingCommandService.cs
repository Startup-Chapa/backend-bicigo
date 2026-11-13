using backend_bicigo.Billing.Application.Errors;
using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Billing.Domain.Model.Commands;
using backend_bicigo.Billing.Domain.Model.Queries;
using backend_bicigo.Shared.Application.Patterns;

namespace backend_bicigo.Billing.Application.Services;

/// <summary>Command service contract for Billing writes (subscriptions, payments, payment methods).</summary>
public interface IBillingCommandService
{
    /// <summary>Handles subscription creation.</summary>
    Task<Result<Subscription, CreateBillingError>> Handle(CreateSubscriptionCommand command, CancellationToken cancellationToken = default);

    /// <summary>Handles payment creation.</summary>
    Task<Result<Payment, CreateBillingError>> Handle(CreatePaymentCommand command, CancellationToken cancellationToken = default);

    /// <summary>Handles payment-method creation.</summary>
    Task<Result<PaymentMethod, CreateBillingError>> Handle(CreatePaymentMethodCommand command, CancellationToken cancellationToken = default);
}
