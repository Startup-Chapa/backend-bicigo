using System.ComponentModel.DataAnnotations;

namespace backend_bicigo.Trip.Interfaces.REST.Resources;

/// <summary>Payload used to finish a trip and calculate its final cost.</summary>
public sealed class FinishTripResource
{
    /// <summary>Owner reference. Must match the trip owner.</summary>
    [Required] public string UserId { get; set; } = string.Empty;

    /// <summary>Recorded distance in kilometres.</summary>
    [Range(typeof(decimal), "0", "1000000")] public decimal DistanceKm { get; set; }

    /// <summary>Price per kilometre in PEN.</summary>
    [Range(typeof(decimal), "0", "1000000")] public decimal PricePerKm { get; set; }

    /// <summary>Whether the user has a premium plan.</summary>
    public bool IsPremium { get; set; }

    /// <summary>Optional finish timestamp in ISO-8601 format.</summary>
    public string? FinishedAt { get; set; }
}
