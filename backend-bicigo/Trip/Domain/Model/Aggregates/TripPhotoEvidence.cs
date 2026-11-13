using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Trip.Domain.Model.Aggregates;

/// <summary>
///     Photo evidence of a trip return (step 3 dropzone in trip-map-view).
/// </summary>
public class TripPhotoEvidence : IAuditableEntity
{
    /// <summary>Protected constructor for EF Core.</summary>
    protected TripPhotoEvidence()
    {
        TripId = null!;
        PhotoUrl = null!;
        CapturedAt = string.Empty;
    }

    /// <summary>Creates a photo evidence record.</summary>
    public TripPhotoEvidence(string tripId, string photoUrl, string? capturedAt = null)
    {
        if (string.IsNullOrWhiteSpace(tripId)) throw new ArgumentException("TripId is required.", nameof(tripId));
        if (string.IsNullOrWhiteSpace(photoUrl)) throw new ArgumentException("PhotoUrl is required.", nameof(photoUrl));
        TripId = tripId.Trim();
        PhotoUrl = photoUrl.Trim();
        CapturedAt = capturedAt ?? DateTime.UtcNow.ToString("o");
    }

    /// <summary>Server id.</summary>
    public int Id { get; private set; }

    /// <summary>Trip reference.</summary>
    public string TripId { get; private set; }

    /// <summary>Photo URL (or storage key).</summary>
    public string PhotoUrl { get; private set; }

    /// <summary>Captured timestamp ISO.</summary>
    public string CapturedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}
