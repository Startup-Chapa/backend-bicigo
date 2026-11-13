using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Trip.Domain.Model.Aggregates;

/// <summary>
///     Route point of a trip (normalized pathCoordinates from trip-map-view).
/// </summary>
public class TripRoutePoint : IAuditableEntity
{
    /// <summary>Protected constructor for EF Core.</summary>
    protected TripRoutePoint()
    {
        TripId = null!;
        RecordedAt = string.Empty;
    }

    /// <summary>Creates a route point.</summary>
    public TripRoutePoint(string tripId, double latitude, double longitude, int sequence, string? recordedAt = null)
    {
        if (string.IsNullOrWhiteSpace(tripId)) throw new ArgumentException("TripId is required.", nameof(tripId));
        if (sequence < 0) throw new ArgumentException("Sequence cannot be negative.", nameof(sequence));
        if (latitude is < -90 or > 90) throw new ArgumentException("Latitude must be between -90 and 90.", nameof(latitude));
        if (longitude is < -180 or > 180) throw new ArgumentException("Longitude must be between -180 and 180.", nameof(longitude));
        TripId = tripId.Trim();
        Latitude = latitude;
        Longitude = longitude;
        Sequence = sequence;
        RecordedAt = recordedAt ?? DateTime.UtcNow.ToString("o");
    }

    /// <summary>Server id.</summary>
    public int Id { get; private set; }

    /// <summary>Trip reference.</summary>
    public string TripId { get; private set; }

    /// <summary>Latitude.</summary>
    public double Latitude { get; private set; }

    /// <summary>Longitude.</summary>
    public double Longitude { get; private set; }

    /// <summary>Order in the path.</summary>
    public int Sequence { get; private set; }

    /// <summary>Recorded timestamp ISO.</summary>
    public string RecordedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}
