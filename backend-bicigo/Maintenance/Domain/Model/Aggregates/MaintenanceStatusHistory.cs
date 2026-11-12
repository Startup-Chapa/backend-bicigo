using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Maintenance.Domain.Model.Aggregates;

/// <summary>
///     Maintenance status history (audit of AVAILABLE ↔ MAINTENANCE transitions).
/// </summary>
public class MaintenanceStatusHistory : IAuditableEntity
{
    /// <summary>Protected constructor for EF Core.</summary>
    protected MaintenanceStatusHistory()
    {
        BikeId = null!;
        PreviousStatus = null!;
        NewStatus = null!;
        ChangedAt = string.Empty;
    }

    /// <summary>Creates a history record.</summary>
    public MaintenanceStatusHistory(int maintenanceReportId, string bikeId, string previousStatus, string newStatus, string? changedAt = null)
    {
        if (string.IsNullOrWhiteSpace(bikeId)) throw new ArgumentException("BikeId is required.", nameof(bikeId));
        if (string.IsNullOrWhiteSpace(previousStatus)) throw new ArgumentException("PreviousStatus is required.", nameof(previousStatus));
        if (string.IsNullOrWhiteSpace(newStatus)) throw new ArgumentException("NewStatus is required.", nameof(newStatus));
        MaintenanceReportId = maintenanceReportId;
        BikeId = bikeId.Trim();
        PreviousStatus = previousStatus.Trim().ToUpperInvariant();
        NewStatus = newStatus.Trim().ToUpperInvariant();
        ChangedAt = changedAt ?? DateTime.UtcNow.ToString("o");
    }

    
    public int Id { get; private set; }

    /// <summary>Report id.</summary>
    public int MaintenanceReportId { get; private set; }

    /// <summary>Bike reference.</summary>
    public string BikeId { get; private set; }

    /// <summary>Previous bike status.</summary>
    public string PreviousStatus { get; private set; }

    /// <summary>New bike status.</summary>
    public string NewStatus { get; private set; }

    /// <summary>Changed timestamp ISO.</summary>
    public string ChangedAt { get; private set; }
    
    public DateTimeOffset? CreatedAt { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
}
