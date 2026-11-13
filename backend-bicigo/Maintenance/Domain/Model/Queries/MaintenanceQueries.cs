namespace backend_bicigo.Maintenance.Domain.Model.Queries;

/// <summary>Query to fetch a maintenance report by id.</summary>
/// <param name="Id">Report id.</param>
public record GetMaintenanceReportByIdQuery(int Id);