using backend_bicigo.Maintenance.Domain.Model.Aggregates;
using backend_bicigo.Maintenance.Interfaces.REST.Resources;

namespace backend_bicigo.Maintenance.Interfaces.REST.Transform;

/// <summary>Assembles MaintenanceStatusHistory resources.</summary>
public static class MaintenanceStatusHistoryAssembler
{
    /// <summary>Assembles a resource from an entity.</summary>
    /// <param name="entity">The history record.</param>
    /// <returns>The resource.</returns>
    public static MaintenanceStatusHistoryResource ToResourceFromEntity(MaintenanceStatusHistory entity) => new()
    {
        Id = entity.Id,
        MaintenanceReportId = entity.MaintenanceReportId,
        BikeId = entity.BikeId,
        PreviousStatus = entity.PreviousStatus,
        NewStatus = entity.NewStatus,
        ChangedAt = entity.ChangedAt
    };
}