using System.ComponentModel.DataAnnotations;

namespace backend_bicigo.Fleet.Interfaces.REST.Resources;

/// <summary>Create bicycle payload (POST /bicycles).</summary>
public sealed class CreateBicycleResource
{
    /// <summary>Serial number.</summary>
    [Required] public string SerialNumber { get; set; } = string.Empty;

    /// <summary>Business code.</summary>
    [Required] public string BikeCode { get; set; } = string.Empty;

    /// <summary>QR snapshot.</summary>
    [Required] public string QrCode { get; set; } = string.Empty;

    /// <summary>Model.</summary>
    [Required] public string Model { get; set; } = string.Empty;

    /// <summary>Optional initial bike-point id.</summary>
    public int? CurrentBikePointId { get; set; }
}
