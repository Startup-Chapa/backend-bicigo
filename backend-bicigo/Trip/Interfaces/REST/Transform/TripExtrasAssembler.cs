using backend_bicigo.Trip.Domain.Model.Aggregates;
using backend_bicigo.Trip.Interfaces.REST.Resources;

namespace backend_bicigo.Trip.Interfaces.REST.Transform;

/// <summary>Assembles Trip extra resources.</summary>
public static class TripExtrasAssembler
{
    /// <summary>Assembles a TripUser resource.</summary>
    /// <param name="entity">The trip user.</param>
    /// <returns>The resource.</returns>
    public static TripUserResource ToTripUserFromEntity(TripUser entity) => new()
    {
        Id = entity.Id,
        UserId = entity.UserId,
        Name = entity.Name,
        Email = entity.Email,
        Role = entity.Role,
        Active = entity.Active
    };

    /// <summary>Assembles a route point resource.</summary>
    /// <param name="entity">The route point.</param>
    /// <returns>The resource.</returns>
    public static TripRoutePointResource ToRoutePointFromEntity(TripRoutePoint entity) => new()
    {
        Id = entity.Id,
        TripId = entity.TripId,
        Latitude = entity.Latitude,
        Longitude = entity.Longitude,
        Sequence = entity.Sequence,
        RecordedAt = entity.RecordedAt
    };

    /// <summary>Assembles a photo evidence resource.</summary>
    /// <param name="entity">The photo evidence.</param>
    /// <returns>The resource.</returns>
    public static TripPhotoEvidenceResource ToPhotoFromEntity(TripPhotoEvidence entity) => new()
    {
        Id = entity.Id,
        TripId = entity.TripId,
        PhotoUrl = entity.PhotoUrl,
        CapturedAt = entity.CapturedAt
    };
}
