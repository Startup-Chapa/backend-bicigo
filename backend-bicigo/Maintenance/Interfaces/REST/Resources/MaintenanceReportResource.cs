namespace backend_bicigo.Maintenance.Interfaces.REST.Resources;

/// <summary>
///     Response payload for MaintenanceReport.
/// </summary>
public sealed class MaintenanceReportResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>Business report id.</summary>
    public string ReportId { get; set; } = string.Empty;

    /// <summary>Bicycle reference.</summary>
    public string BikeId { get; set; } = string.Empty;

    /// <summary>Problem type.</summary>
    public string ProblemType { get; set; } = string.Empty;

    /// <summary>Description.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Bike status AVAILABLE|MAINTENANCE.</summary>
    public string Status { get; set; } = "AVAILABLE";

    /// <summary>Created timestamp ISO.</summary>
    public string CreatedAt { get; set; } = string.Empty;
}