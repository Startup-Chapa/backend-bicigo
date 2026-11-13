namespace backend_bicigo.Trip.Interfaces.REST.Resources;

/// <summary>
///     Response payload for QrValidation (matches db.json/qrValidations).
/// </summary>
public sealed class QrValidationResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>Business validation id.</summary>
    public string ValidationId { get; set; } = string.Empty;

    /// <summary>Trip reference.</summary>
    public string TripId { get; set; } = string.Empty;

    /// <summary>Bicycle reference.</summary>
    public string BicycleId { get; set; } = string.Empty;

    /// <summary>Scanned QR snapshot.</summary>
    public string QrCodeSnapshot { get; set; } = string.Empty;

    /// <summary>Validated timestamp ISO (nullable).</summary>
    public string? ValidatedAt { get; set; }

    /// <summary>Result PENDING|VALID|INVALID.</summary>
    public string Result { get; set; } = "PENDING";
}
