using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Billing.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     EF Core repository for Subscription persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class SubscriptionRepository(AppDbContext context) : BaseRepository<Subscription>(context)
{
    /// <summary>Find active subscriptions for a user.</summary>
    /// <param name="userId">User id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Active subscriptions.</returns>
    public async Task<IEnumerable<Subscription>> FindActiveByUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Subscription>()
            .Where(s => s.UserId == userId && s.Status == "ACTIVE")
            .ToListAsync(cancellationToken);
    }
}
