namespace backend_bicigo.Maintenance.Domain.Model.Commands;

/// <summary>Command to create a maintenance report.</summary>
public record CreateMaintenanceReportCommand(
    string BikeId,
    string ProblemType,
    string Description,
    string? ReportId = null);

/// <summary>Command to update a bike status via maintenance.</summary>
public record UpdateBikeStatusCommand(
    string BikeId,
    string Status);