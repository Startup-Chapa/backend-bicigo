using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Shared.Domain.Repositories;

namespace backend_bicigo.Billing.Domain.Repositories;

/// <summary>The Subscription repository contract.</summary>
public interface IBillingRepository : IBaseRepository<Subscription>
{
    /// <summary>Find active subscriptions for a user (used by findPayPerUseSubscription).</summary>
    /// <param name="userId">User id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Active subscriptions for the user.</returns>
    Task<IEnumerable<Subscription>> FindActiveByUserAsync(int userId, CancellationToken cancellationToken = default);
}
