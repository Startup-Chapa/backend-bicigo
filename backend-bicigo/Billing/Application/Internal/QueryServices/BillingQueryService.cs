using backend_bicigo.Billing.Application.Services;
using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Billing.Domain.Model.Queries;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Billing.Application.Internal.QueryServices;

/// <summary>
///     Billing query service for reads.
/// </summary>
/// <param name="context">EF Core context.</param>
public class BillingQueryService(AppDbContext context) : IBillingQueryService
{
    /// <inheritdoc />
    public async Task<Plan?> Handle(GetPlanByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await context.Set<Plan>().FindAsync([query.Id], cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Subscription>> Handle(GetSubscriptionsByUserQuery query, CancellationToken cancellationToken = default)
    {
        return await context.Set<Subscription>()
            .Where(s => s.UserId == query.UserId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Payment>> Handle(GetPaymentsByUserQuery query, CancellationToken cancellationToken = default)
    {
        return await context.Set<Payment>()
            .Where(p => p.UserId == query.UserId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PaymentMethod>> Handle(GetPaymentMethodsByUserQuery query, CancellationToken cancellationToken = default)
    {
        return await context.Set<PaymentMethod>()
            .Where(p => p.UserId == query.UserId)
            .ToListAsync(cancellationToken);
    }
}
