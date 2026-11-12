namespace backend_bicigo.Maintenance.Domain.Model.Commands;

/// <summary>Command to record a maintenance status change.</summary>
public record CreateMaintenanceStatusHistoryCommand(
    int MaintenanceReportId,
    string BikeId,
    string PreviousStatus,
    string NewStatus,
    string? ChangedAt = null);