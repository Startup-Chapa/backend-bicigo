using System.ComponentModel.DataAnnotations;

namespace backend_bicigo.Trip.Interfaces.REST.Resources;

/// <summary>Create QR validation payload (POST /qr-validations).</summary>
public sealed class CreateQrValidationResource
{
    /// <summary>Business validation id.</summary>
    [Required] public string ValidationId { get; set; } = string.Empty;

    /// <summary>Trip reference.</summary>
    [Required] public string TripId { get; set; } = string.Empty;

    /// <summary>Bicycle reference.</summary>
    [Required] public string BicycleId { get; set; } = string.Empty;

    /// <summary>Scanned QR snapshot.</summary>
    [Required] public string QrCodeSnapshot { get; set; } = string.Empty;

    /// <summary>Expected QR (optional; validates immediately when present).</summary>
    public string? ExpectedQrCode { get; set; }
}
