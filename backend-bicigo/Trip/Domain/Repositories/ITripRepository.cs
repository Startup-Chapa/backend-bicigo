using backend_bicigo.Shared.Domain.Repositories;
using backend_bicigo.Trip.Domain.Model.Aggregates;

namespace backend_bicigo.Trip.Domain.Repositories;

/// <summary>The Trip repository contract.</summary>
public interface ITripRepository : IBaseRepository<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>
{
    /// <summary>Find trips by user (frontend filters by currentUser.id).</summary>
    /// <param name="userId">User reference.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Trips for the user.</returns>
    Task<IEnumerable<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>> FindByUserAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>Find the active (IN_PROGRESS) trip for a user, if any.</summary>
    /// <param name="userId">User reference.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The active trip or null.</returns>
    Task<backend_bicigo.Trip.Domain.Model.Aggregates.Trip?> FindActiveByUserAsync(string userId, CancellationToken cancellationToken = default);
}
