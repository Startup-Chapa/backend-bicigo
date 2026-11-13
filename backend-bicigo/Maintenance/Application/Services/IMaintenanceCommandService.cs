using backend_bicigo.Maintenance.Application.Errors;
using backend_bicigo.Maintenance.Domain.Model.Aggregates;
using backend_bicigo.Maintenance.Domain.Model.Commands;
using backend_bicigo.Shared.Application.Patterns;

namespace backend_bicigo.Maintenance.Application.Services;

/// <summary>Command service contract for Maintenance writes.</summary>
public interface IMaintenanceCommandService
{
    /// <summary>Handles report creation.</summary>
    Task<Result<MaintenanceReport, CreateMaintenanceError>> Handle(CreateMaintenanceReportCommand command, CancellationToken cancellationToken = default);
    
    Task<Result<bool, CreateMaintenanceError>> Handle(UpdateBikeStatusCommand command, CancellationToken cancellationToken = default);
}