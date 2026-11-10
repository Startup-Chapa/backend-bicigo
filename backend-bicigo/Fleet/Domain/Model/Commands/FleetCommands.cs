namespace backend_bicigo.Fleet.Domain.Model.Commands;

/// <summary>Command to register a bicycle.</summary>
public record RegisterBicycleCommand(
    string SerialNumber,
    string BikeCode,
    string QrCode,
    string Model,
    int? CurrentBikePointId = null);

/// <summary>Command to update a bicycle.</summary>
public record UpdateBicycleCommand(
    string? SerialNumber = null,
    string? BikeCode = null,
    string? QrCode = null,
    string? Model = null,
    string? Status = null,
    int? CurrentBikePointId = null);

/// <summary>Command to register a bike point.</summary>
public record RegisterBikePointCommand(
    string Name,
    string District,
    string Address,
    double Latitude,
    double Longitude,
    int Capacity);

/// <summary>Command to update a bike point.</summary>
public record UpdateBikePointCommand(
    string? Name = null,
    string? District = null,
    string? Address = null,
    double? Latitude = null,
    double? Longitude = null,
    int? Capacity = null,
    string? Status = null);
