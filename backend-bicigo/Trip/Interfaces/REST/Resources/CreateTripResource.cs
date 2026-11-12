using System.ComponentModel.DataAnnotations;

namespace backend_bicigo.Trip.Interfaces.REST.Resources;

/// <summary>Create trip payload (POST /trips).</summary>
public sealed class CreateTripResource
{
    /// <summary>Business trip id.</summary>
    [Required] public string TripId { get; set; } = string.Empty;

    /// <summary>User reference.</summary>
    [Required] public string UserId { get; set; } = string.Empty;

    /// <summary>Bicycle reference.</summary>
    [Required] public string BicycleId { get; set; } = string.Empty;

    /// <summary>Started timestamp ISO (optional).</summary>
    public string? StartedAt { get; set; }
}
