namespace backend_bicigo.Trip.Domain.Model.Commands;

/// <summary>Command to create a trip.</summary>
public record CreateTripCommand(
    string TripId,
    string UserId,
    string BicycleId,
    string? StartedAt = null);

/// <summary>Command to update a trip (status, distance, cost).</summary>
public record UpdateTripCommand(
    string? UserId = null,
    string? BicycleId = null,
    string? StartedAt = null,
    string? FinishedAt = null,
    decimal? DistanceKm = null,
    string? Status = null,
    decimal? TotalCost = null);

/// <summary>Command to create a QR validation.</summary>
public record CreateQrValidationCommand(
    string ValidationId,
    string TripId,
    string BicycleId,
    string QrCodeSnapshot,
    string? ExpectedQrCode = null);
