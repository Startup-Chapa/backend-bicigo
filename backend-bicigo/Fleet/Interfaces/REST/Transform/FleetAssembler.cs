using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Fleet.Domain.Model.Commands;
using backend_bicigo.Fleet.Domain.Model.ValueObjects;
using backend_bicigo.Fleet.Interfaces.REST.Resources;

namespace backend_bicigo.Fleet.Interfaces.REST.Transform;

/// <summary>
///     Assembles Fleet resources and commands.
/// </summary>
public static class FleetAssembler
{
    /// <summary>Assembles a BicycleResource from a Bicycle.</summary>
    /// <param name="entity">The bicycle.</param>
    /// <returns>The resource.</returns>
    public static BicycleResource ToBicycleFromEntity(Bicycle entity) => new()
    {
        Id = entity.Id,
        SerialNumber = entity.SerialNumber,
        BikeCode = entity.BikeCode,
        QrCode = entity.QrCode,
        Model = entity.Model,
        Status = entity.Status,
        CurrentBikePointId = entity.CurrentBikePointId
    };

    /// <summary>Assembles a BikePointResource from a BikePoint.</summary>
    /// <param name="entity">The bike point.</param>
    /// <returns>The resource.</returns>
    public static BikePointResource ToBikePointFromEntity(BikePoint entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        District = entity.District,
        Address = entity.Address,
        Latitude = entity.Latitude,
        Longitude = entity.Longitude,
        Capacity = entity.Capacity,
        AvailableSlots = entity.AvailableSlots,
        CurrentBicyclesCount = entity.CurrentBicyclesCount,
        Status = entity.Status
    };

    /// <summary>Assembles a register-bicycle command from a create resource.</summary>
    /// <param name="resource">The create resource.</param>
    /// <returns>The command.</returns>
    public static RegisterBicycleCommand ToRegisterBicycleCommand(CreateBicycleResource resource) =>
        new(
            SerialNumber: resource.SerialNumber.Trim(),
            BikeCode: resource.BikeCode.Trim(),
            QrCode: resource.QrCode.Trim(),
            Model: resource.Model.Trim(),
            CurrentBikePointId: resource.CurrentBikePointId);

    /// <summary>Assembles a register-bike-point command from a create resource.</summary>
    /// <param name="resource">The create resource.</param>
    /// <returns>The command.</returns>
    public static RegisterBikePointCommand ToRegisterBikePointCommand(CreateBikePointResource resource) =>
        new(
            Name: resource.Name.Trim(),
            District: resource.District.Trim(),
            Address: resource.Address.Trim(),
            Latitude: resource.Latitude,
            Longitude: resource.Longitude,
            Capacity: resource.Capacity);

    /// <summary>Validates a bicycle status string.</summary>
    /// <param name="status">Status string.</param>
    /// <returns>The normalized status.</returns>
    public static string ToBicycleStatus(string status) => new BicycleStatus(status).Value;

    /// <summary>Validates a bike-point status string.</summary>
    /// <param name="status">Status string.</param>
    /// <returns>The normalized status.</returns>
    public static string ToBikePointStatus(string status) => new BikePointStatus(status).Value;
}
