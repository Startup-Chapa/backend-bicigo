namespace backend_bicigo.Trip.Domain.Model.Commands;

/// <summary>Command to create a trip user.</summary>
public record CreateTripUserCommand(string UserId, string Name, string Email, string? Role = null, bool Active = true);

/// <summary>Command to create a route point.</summary>
public record CreateTripRoutePointCommand(string TripId, double Latitude, double Longitude, int Sequence, string? RecordedAt = null);

/// <summary>Command to create a photo evidence.</summary>
public record CreateTripPhotoEvidenceCommand(string TripId, string PhotoUrl, string? CapturedAt = null);
