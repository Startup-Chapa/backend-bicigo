namespace backend_bicigo.Trip.Interfaces.REST.Resources;

/// <summary>Photo evidence response.</summary>
public sealed class TripPhotoEvidenceResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>Trip reference.</summary>
    public string TripId { get; set; } = string.Empty;

    /// <summary>Photo URL.</summary>
    public string PhotoUrl { get; set; } = string.Empty;

    /// <summary>Captured timestamp ISO.</summary>
    public string CapturedAt { get; set; } = string.Empty;
}
