namespace backend_bicigo.Fleet.Interfaces.REST.Resources;

/// <summary>
///     Response payload for Bicycle (matches db.json/bicycles).
/// </summary>
/// <remarks>
///     The frontend BicycleAssembler accepts <c>id</c> or <c>bicycleId</c>; we expose both for compatibility.
/// </remarks>
public sealed class BicycleResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>Alias for id (frontend compatibility).</summary>
    public int BicycleId => Id;

    /// <summary>Serial number.</summary>
    public string SerialNumber { get; set; } = string.Empty;

    /// <summary>Business code.</summary>
    public string BikeCode { get; set; } = string.Empty;

    /// <summary>QR snapshot.</summary>
    public string QrCode { get; set; } = string.Empty;

    /// <summary>Model.</summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>Status AVAILABLE|IN_USE|MAINTENANCE|DISABLED.</summary>
    public string Status { get; set; } = "AVAILABLE";

    /// <summary>Current bike-point id (nullable).</summary>
    public int? CurrentBikePointId { get; set; }
}
