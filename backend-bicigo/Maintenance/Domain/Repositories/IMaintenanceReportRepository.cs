using backend_bicigo.Maintenance.Domain.Model.Aggregates;
using backend_bicigo.Shared.Domain.Repositories;

namespace backend_bicigo.Maintenance.Domain.Repositories;

/// <summary>The Maintenance report repository contract.</summary>
public interface IMaintenanceReportRepository : IBaseRepository<MaintenanceReport>
{
    /// <summary>Find reports by bike id.</summary>
    /// <param name="bikeId">Bike reference.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Reports for the bike.</returns>
    Task<IEnumerable<MaintenanceReport>> FindByBikeIdAsync(string bikeId, CancellationToken cancellationToken = default);
}