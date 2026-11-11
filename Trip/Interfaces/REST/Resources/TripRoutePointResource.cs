namespace backend_bicigo.Trip.Interfaces.REST.Resources;

/// <summary>Route point response.</summary>
public sealed class TripRoutePointResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>Trip reference.</summary>
    public string TripId { get; set; } = string.Empty;

    /// <summary>Latitude.</summary>
    public double Latitude { get; set; }

    /// <summary>Longitude.</summary>
    public double Longitude { get; set; }

    /// <summary>Order in the path.</summary>
    public int Sequence { get; set; }

    /// <summary>Recorded timestamp ISO.</summary>
    public string RecordedAt { get; set; } = string.Empty;
}
