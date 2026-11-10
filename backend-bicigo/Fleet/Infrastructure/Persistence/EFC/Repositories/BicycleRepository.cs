using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Fleet.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     EF Core repository for Bicycle persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class BicycleRepository(AppDbContext context)
    : BaseRepository<Bicycle>(context), Domain.Repositories.IBicycleRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<Bicycle>> FindByBikePointAsync(int bikePointId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Bicycle>()
            .Where(b => b.CurrentBikePointId == bikePointId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Bicycle?> FindByBikeCodeAsync(string bikeCode, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Bicycle>()
            .FirstOrDefaultAsync(b => b.BikeCode == bikeCode.Trim(), cancellationToken);
    }
}
