using backend_bicigo.Maintenance.Application.Services;
using backend_bicigo.Maintenance.Domain.Model.Aggregates;
using backend_bicigo.Maintenance.Domain.Model.Queries;
using backend_bicigo.Maintenance.Domain.Repositories;

namespace backend_bicigo.Maintenance.Application.Internal.QueryServices;

/// <summary>
///     Maintenance query service.
/// </summary>
/// <param name="reportRepository">Repository for reports.</param>
public class MaintenanceQueryService(IMaintenanceReportRepository reportRepository) : IMaintenanceQueryService
{
    /// <inheritdoc />
    public async Task<MaintenanceReport?> Handle(GetMaintenanceReportByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await reportRepository.FindByIdAsync(query.Id, cancellationToken);
    }
}