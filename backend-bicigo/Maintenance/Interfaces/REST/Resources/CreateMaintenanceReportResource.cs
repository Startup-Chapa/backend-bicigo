using System.ComponentModel.DataAnnotations;

namespace backend_bicigo.Maintenance.Interfaces.REST.Resources;

/// <summary>Create maintenance report payload (POST /maintenance-reports).</summary>
public sealed class CreateMaintenanceReportResource
{
    [Required] [MaxLength(64)] public string BikeId { get; set; } = string.Empty;
    [Required] public string ProblemType { get; set; } = string.Empty;
    [Required] [MinLength(10)] [MaxLength(1000)] public string Description { get; set; } = string.Empty;
    public string? ReportId { get; set; }
}