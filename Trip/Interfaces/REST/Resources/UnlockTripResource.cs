using System.ComponentModel.DataAnnotations;

namespace backend_bicigo.Trip.Interfaces.REST.Resources;

/// <summary>Payload used to validate the bicycle QR code and unlock a pending trip.</summary>
public sealed class UnlockTripResource
{
    /// <summary>Owner reference. Must match the trip owner.</summary>
    [Required] public string UserId { get; set; } = string.Empty;

    /// <summary>QR value scanned from the bicycle.</summary>
    [Required] public string QrCodeSnapshot { get; set; } = string.Empty;
}
