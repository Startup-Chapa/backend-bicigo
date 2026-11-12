using backend_bicigo.Maintenance.Domain.Model.Aggregates;
using backend_bicigo.Maintenance.Domain.Model.Commands;
using backend_bicigo.Maintenance.Interfaces.REST.Resources;

namespace backend_bicigo.Maintenance.Interfaces.REST.Transform;

/// <summary>
///     Assembles Maintenance reports and commands.
/// </summary>
public static class MaintenanceAssembler
{
    /// <summary>Assembles a MaintenanceReportResource from a MaintenanceReport.</summary>
    /// <param name="entity">The report.</param>
    /// <returns>The resource.</returns>
    public static MaintenanceReportResource ToResourceFromEntity(MaintenanceReport entity) => new()
    {
        Id = entity.Id,
        ReportId = entity.ReportId,
        BikeId = entity.BikeId,
        ProblemType = entity.ProblemType,
        Description = entity.Description,
        Status = entity.Status,
        CreatedAt = entity.CreatedAtUtc
    };

    /// <summary>Assembles a create-report command from a create resource.</summary>
    /// <param name="resource">The create resource.</param>
    /// <returns>The command.</returns>
    public static CreateMaintenanceReportCommand ToCommandFromResource(CreateMaintenanceReportResource resource) =>
        new(
            BikeId: resource.BikeId.Trim(),
            ProblemType: resource.ProblemType.Trim().ToUpperInvariant(),
            Description: resource.Description.Trim(),
            ReportId: resource.ReportId?.Trim());
}