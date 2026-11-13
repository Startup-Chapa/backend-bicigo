using backend_bicigo.Trip.Domain.Model.Aggregates;
using backend_bicigo.Trip.Domain.Model.Commands;
using backend_bicigo.Trip.Domain.Model.ValueObjects;
using backend_bicigo.Trip.Interfaces.REST.Resources;

namespace backend_bicigo.Trip.Interfaces.REST.Transform;

/// <summary>
///     Assembles Trip resources and commands.
/// </summary>
public static class TripAssembler
{
    /// <summary>Assembles a TripResource from a Trip.</summary>
    /// <param name="entity">The trip.</param>
    /// <returns>The resource.</returns>
    public static TripResource ToTripFromEntity(backend_bicigo.Trip.Domain.Model.Aggregates.Trip entity) => new()
    {
        Id = entity.Id,
        TripId = entity.TripId,
        UserId = entity.UserId,
        BicycleId = entity.BicycleId,
        StartedAt = entity.StartedAt,
        FinishedAt = entity.FinishedAt,
        DistanceKm = entity.DistanceKm,
        Status = entity.Status,
        TotalCost = entity.TotalCost
    };

    /// <summary>Assembles a QrValidationResource from a QrValidation.</summary>
    /// <param name="entity">The validation.</param>
    /// <returns>The resource.</returns>
    public static QrValidationResource ToQrFromEntity(QrValidation entity) => new()
    {
        Id = entity.Id,
        ValidationId = entity.ValidationId,
        TripId = entity.TripId,
        BicycleId = entity.BicycleId,
        QrCodeSnapshot = entity.QrCodeSnapshot,
        ValidatedAt = entity.ValidatedAt,
        Result = entity.Result
    };

    /// <summary>Assembles a create-trip command.</summary>
    /// <param name="resource">The create resource.</param>
    /// <returns>The command.</returns>
    public static CreateTripCommand ToCreateTripCommand(CreateTripResource resource) =>
        new(resource.TripId.Trim(), resource.UserId.Trim(), resource.BicycleId.Trim(), resource.StartedAt);

    /// <summary>Assembles a finish-trip command.</summary>
    /// <param name="id">Server trip id.</param>
    /// <param name="resource">The finish resource.</param>
    /// <returns>The command.</returns>
    public static FinishTripCommand ToFinishTripCommand(int id, FinishTripResource resource) =>
        new(id, resource.DistanceKm ?? 0m);

    /// <summary>Assembles a create-QR command.</summary>
    /// <param name="resource">The create resource.</param>
    /// <returns>The command.</returns>
    public static CreateQrValidationCommand ToCreateQrCommand(CreateQrValidationResource resource) =>
        new(resource.ValidationId.Trim(), resource.TripId.Trim(), resource.BicycleId.Trim(), resource.QrCodeSnapshot.Trim(), resource.ExpectedQrCode);

    /// <summary>Validates a trip status.</summary>
    /// <param name="status">Status string.</param>
    /// <returns>The normalized status.</returns>
    public static string ToTripStatus(string status) => new TripStatus(status).Value;

    /// <summary>Validates a QR result.</summary>
    /// <param name="result">Result string.</param>
    /// <returns>The normalized result.</returns>
    public static string ToQrResult(string result) => new QrValidationResult(result).Value;
}
