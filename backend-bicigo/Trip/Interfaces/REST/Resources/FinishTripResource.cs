using System.ComponentModel.DataAnnotations;

namespace backend_bicigo.Trip.Interfaces.REST.Resources;

/// <summary>
///     Finish trip payload (POST /trips/{id}/finish).
///     Only the distance is accepted from the client; finishedAt and totalCost are computed server-side.
/// </summary>
public sealed class FinishTripResource
{
    /// <summary>Distance ridden in km (&gt;= 0). Required.</summary>
    [Required]
    [Range(0, 1000000)]
    public decimal? DistanceKm { get; set; }
}