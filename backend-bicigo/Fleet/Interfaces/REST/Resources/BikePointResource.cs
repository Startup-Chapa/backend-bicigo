namespace backend_bicigo.Fleet.Interfaces.REST.Resources;

/// <summary>
///     Response payload for BikePoint (matches db.json/bike-points).
/// </summary>
public sealed class BikePointResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>Alias for id (frontend compatibility).</summary>
    public int BikePointId => Id;

    /// <summary>Station name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>District.</summary>
    public string District { get; set; } = string.Empty;

    /// <summary>Address.</summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>Latitude.</summary>
    public double Latitude { get; set; }

    /// <summary>Longitude.</summary>
    public double Longitude { get; set; }

    /// <summary>Total capacity.</summary>
    public int Capacity { get; set; }

    /// <summary>Free slots (calculated).</summary>
    public int AvailableSlots { get; set; }

    /// <summary>Docked count.</summary>
    public int CurrentBicyclesCount { get; set; }

    /// <summary>Status OPERATIONAL|FULL|MAINTENANCE|DISABLED.</summary>
    public string Status { get; set; } = "OPERATIONAL";
}
