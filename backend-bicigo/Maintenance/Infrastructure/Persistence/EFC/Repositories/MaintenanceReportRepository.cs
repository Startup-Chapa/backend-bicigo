using backend_bicigo.Maintenance.Domain.Model.Aggregates;
using backend_bicigo.Maintenance.Domain.Repositories;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Maintenance.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     EF Core repository for MaintenanceReport persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class MaintenanceReportRepository(AppDbContext context)
    : BaseRepository<MaintenanceReport>(context), IMaintenanceReportRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<MaintenanceReport>> FindByBikeIdAsync(string bikeId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<MaintenanceReport>()
            .Where(m => m.BikeId == bikeId.Trim())
            .ToListAsync(cancellationToken);
    }
}