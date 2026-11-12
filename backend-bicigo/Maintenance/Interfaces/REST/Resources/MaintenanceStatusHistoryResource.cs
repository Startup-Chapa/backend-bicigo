using backend_bicigo.Maintenance.Domain.Model.Aggregates;

namespace backend_bicigo.Maintenance.Interfaces.REST.Resources;

/// <summary>Maintenance status history response.</summary>
public sealed class MaintenanceStatusHistoryResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>Report id.</summary>
    public int MaintenanceReportId { get; set; }

    /// <summary>Bike reference.</summary>
    public string BikeId { get; set; } = string.Empty;

    /// <summary>Previous bike status.</summary>
    public string PreviousStatus { get; set; } = string.Empty;

    /// <summary>New bike status.</summary>
    public string NewStatus { get; set; } = string.Empty;

    /// <summary>Changed timestamp ISO.</summary>
    public string ChangedAt { get; set; } = string.Empty;
}