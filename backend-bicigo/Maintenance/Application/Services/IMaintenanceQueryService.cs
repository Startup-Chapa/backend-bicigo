using backend_bicigo.Maintenance.Domain.Model.Aggregates;
using backend_bicigo.Maintenance.Domain.Model.Queries;

namespace backend_bicigo.Maintenance.Application.Services;

/// <summary>Query service contract for Maintenance reads.</summary>
public interface IMaintenanceQueryService
{
    Task<MaintenanceReport?> Handle(GetMaintenanceReportByIdQuery query, CancellationToken cancellationToken = default);
}