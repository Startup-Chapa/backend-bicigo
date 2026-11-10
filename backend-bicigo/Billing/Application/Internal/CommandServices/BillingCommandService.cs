using backend_bicigo.Billing.Application.Errors;
using backend_bicigo.Billing.Application.Services;
using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Billing.Domain.Model.Commands;
using backend_bicigo.Shared.Application.Patterns;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Billing.Application.Internal.CommandServices;

/// <summary>
///     Billing command service: creates subscriptions, payments and payment methods.
/// </summary>
/// <param name="context">EF Core context (same unit of work as the app).</param>
public class BillingCommandService(AppDbContext context) : IBillingCommandService
{
    /// <inheritdoc />
    public async Task<Result<Subscription, CreateBillingError>> Handle(CreateSubscriptionCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = new Subscription(command);
            await context.Set<Subscription>().AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return new Result<Subscription, CreateBillingError>.Success(entity);
        }
        catch (ArgumentException)
        {
            return new Result<Subscription, CreateBillingError>.Failure(CreateBillingError.InvalidInput);
        }
        catch (Exception)
        {
            return new Result<Subscription, CreateBillingError>.Failure(CreateBillingError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<Payment, CreateBillingError>> Handle(CreatePaymentCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = new Payment(command);
            await context.Set<Payment>().AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return new Result<Payment, CreateBillingError>.Success(entity);
        }
        catch (ArgumentException)
        {
            return new Result<Payment, CreateBillingError>.Failure(CreateBillingError.InvalidInput);
        }
        catch (Exception)
        {
            return new Result<Payment, CreateBillingError>.Failure(CreateBillingError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<PaymentMethod, CreateBillingError>> Handle(CreatePaymentMethodCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(command.Token))
                return new Result<PaymentMethod, CreateBillingError>.Failure(CreateBillingError.InvalidInput);
            var entity = new PaymentMethod(command);
            await context.Set<PaymentMethod>().AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return new Result<PaymentMethod, CreateBillingError>.Success(entity);
        }
        catch (ArgumentException)
        {
            return new Result<PaymentMethod, CreateBillingError>.Failure(CreateBillingError.InvalidInput);
        }
        catch (Exception)
        {
            return new Result<PaymentMethod, CreateBillingError>.Failure(CreateBillingError.UnexpectedError);
        }
    }
}
