namespace backend_bicigo.Trip.Interfaces.REST.Resources;

/// <summary>
///     Response payload for Trip (matches db.json/trips).
/// </summary>
public sealed class TripResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>Business trip id.</summary>
    public string TripId { get; set; } = string.Empty;

    /// <summary>User reference.</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>Bicycle reference.</summary>
    public string BicycleId { get; set; } = string.Empty;

    /// <summary>Started timestamp ISO (nullable).</summary>
    public string? StartedAt { get; set; }

    /// <summary>Finished timestamp ISO (nullable).</summary>
    public string? FinishedAt { get; set; }

    /// <summary>Distance km.</summary>
    public decimal DistanceKm { get; set; }

    /// <summary>Status PENDING|IN_PROGRESS|FINISHED.</summary>
    public string Status { get; set; } = "PENDING";

    /// <summary>Total cost.</summary>
    public decimal TotalCost { get; set; }
}
