using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Repositories;
using backend_bicigo.Trip.Domain.Model.Aggregates;
using backend_bicigo.Trip.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Trip.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     EF Core repository for Trip persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class TripRepository(AppDbContext context)
    : BaseRepository<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>(context), ITripRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>> FindByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>()
            .Where(t => t.UserId == userId.Trim())
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<backend_bicigo.Trip.Domain.Model.Aggregates.Trip?> FindActiveByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>()
            .FirstOrDefaultAsync(t => t.UserId == userId.Trim() && t.Status == backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.InProgress, cancellationToken);
    }
}
