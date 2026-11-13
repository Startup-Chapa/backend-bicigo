using backend_bicigo.Trip.Domain.Model.Commands;
using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Trip.Domain.Model.Aggregates;

/// <summary>
///     QR validation aggregate. Mirrors
///     <c>trip-management/domain/model/qr-validation.entity.js</c> and <c>db.json/qrValidations</c>.
/// </summary>
public class QrValidation : IAuditableEntity
{
    /// <summary>Valid validation results (PENDING|VALID|INVALID).</summary>
    public static class Results
    {
        /// <summary>Not validated yet.</summary>
        public const string Pending = "PENDING";
        /// <summary>Matched the expected QR.</summary>
        public const string Valid = "VALID";
        /// <summary>Did not match.</summary>
        public const string Invalid = "INVALID";
    }

    /// <summary>Protected parameterless constructor for EF Core.</summary>
    protected QrValidation()
    {
        ValidationId = null!;
        TripId = null!;
        BicycleId = null!;
        QrCodeSnapshot = null!;
        Result = Results.Pending;
    }

    /// <summary>Creates a validation from a command.</summary>
    /// <param name="command">The creation command.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when required fields are empty.</exception>
    public QrValidation(CreateQrValidationCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (string.IsNullOrWhiteSpace(command.ValidationId)) throw new ArgumentException("ValidationId is required.", nameof(command));
        if (string.IsNullOrWhiteSpace(command.TripId)) throw new ArgumentException("TripId is required.", nameof(command));
        if (string.IsNullOrWhiteSpace(command.BicycleId)) throw new ArgumentException("BicycleId is required.", nameof(command));
        if (string.IsNullOrWhiteSpace(command.QrCodeSnapshot)) throw new ArgumentException("QrCodeSnapshot is required.", nameof(command));
        ValidationId = command.ValidationId.Trim();
        TripId = command.TripId.Trim();
        BicycleId = command.BicycleId.Trim();
        QrCodeSnapshot = command.QrCodeSnapshot.Trim();
        ValidatedAt = null;
        Result = Results.Pending;
    }

    /// <summary>Server-generated id.</summary>
    public int Id { get; private set; }

    /// <summary>Business validation id, e.g. QRV-001.</summary>
    public string ValidationId { get; private set; }

    /// <summary>Trip reference.</summary>
    public string TripId { get; private set; }

    /// <summary>Bicycle reference.</summary>
    public string BicycleId { get; private set; }

    /// <summary>Scanned QR snapshot.</summary>
    public string QrCodeSnapshot { get; private set; }

    /// <summary>Validated timestamp ISO (nullable).</summary>
    public string? ValidatedAt { get; private set; }

    /// <summary>Result PENDING|VALID|INVALID.</summary>
    public string Result { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>Validate against the expected QR code.</summary>
    /// <param name="expectedQrCode">Expected QR from the bicycle.</param>
    public void Validate(string expectedQrCode)
    {
        Result = string.Equals(QrCodeSnapshot, expectedQrCode?.Trim(), StringComparison.Ordinal)
            ? Results.Valid
            : Results.Invalid;
        ValidatedAt = DateTime.UtcNow.ToString("o");
    }
}
