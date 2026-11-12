using backend_bicigo.Fleet.Domain.Model.Commands;
using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Fleet.Domain.Model.Aggregates;

/// <summary>
///     Bicycle aggregate. Mirrors <c>fleet-station-managment/domain/model/bicycle.entity.js</c>
///     and <c>db.json/bicycles</c>.
/// </summary>
public class Bicycle : IAuditableEntity
{
    /// <summary>Valid bicycle statuses (AVAILABLE|IN_USE|MAINTENANCE|DISABLED).</summary>
    public static class Statuses
    {
        /// <summary>Available at a station.</summary>
        public const string Available = "AVAILABLE";
        /// <summary>Currently ridden.</summary>
        public const string InUse = "IN_USE";
        /// <summary>Under maintenance.</summary>
        public const string Maintenance = "MAINTENANCE";
        /// <summary>Disabled.</summary>
        public const string Disabled = "DISABLED";
    }

    /// <summary>Protected parameterless constructor for EF Core.</summary>
    protected Bicycle()
    {
        SerialNumber = null!;
        BikeCode = null!;
        QrCode = null!;
        Model = null!;
        Status = Statuses.Available;
    }

    /// <summary>Creates a bicycle from a register command.</summary>
    /// <param name="command">The registration command.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when serial/bikeCode/qr/model are empty.</exception>
    public Bicycle(RegisterBicycleCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (string.IsNullOrWhiteSpace(command.SerialNumber)) throw new ArgumentException("SerialNumber is required.", nameof(command));
        if (string.IsNullOrWhiteSpace(command.BikeCode)) throw new ArgumentException("BikeCode is required.", nameof(command));
        if (string.IsNullOrWhiteSpace(command.QrCode)) throw new ArgumentException("QrCode is required.", nameof(command));
        if (string.IsNullOrWhiteSpace(command.Model)) throw new ArgumentException("Model is required.", nameof(command));
        SerialNumber = command.SerialNumber.Trim();
        BikeCode = command.BikeCode.Trim();
        QrCode = command.QrCode.Trim();
        Model = command.Model.Trim();
        Status = Statuses.Available;
        CurrentBikePointId = command.CurrentBikePointId;
    }

    /// <summary>Server-generated id (also acts as the numeric id for CRUD).</summary>
    public int Id { get; private set; }

    /// <summary>Serial number, e.g. BG-1042.</summary>
    public string SerialNumber { get; private set; }

    /// <summary>Business code, e.g. BGO-1042.</summary>
    public string BikeCode { get; private set; }

    /// <summary>QR code snapshot, e.g. BICIGO-BGO-1042.</summary>
    public string QrCode { get; private set; }

    /// <summary>Model, e.g. City.</summary>
    public string Model { get; private set; }

    /// <summary>Status AVAILABLE|IN_USE|MAINTENANCE|DISABLED (defaults AVAILABLE).</summary>
    public string Status { get; private set; }

    /// <summary>Current bike-point id (nullable).</summary>
    public int? CurrentBikePointId { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>Unlock the bicycle (AVAILABLE → IN_USE).</summary>
    /// <returns>True when the transition happened (idempotent).</returns>
    public bool Unlock()
    {
        if (Status == Statuses.InUse) return true;
        if (Status != Statuses.Available) return false;
        Status = Statuses.InUse;
        CurrentBikePointId = null;
        return true;
    }

    /// <summary>Update the status to a valid value.</summary>
    /// <param name="status">New status.</param>
    /// <exception cref="ArgumentException">Thrown when status is not a valid value.</exception>
    public void UpdateStatus(string status)
    {
        var normalized = (status ?? string.Empty).Trim().ToUpperInvariant();
        if (normalized is not (Statuses.Available or Statuses.InUse or Statuses.Maintenance or Statuses.Disabled))
            throw new ArgumentException($"Invalid bicycle status '{status}'.", nameof(status));
        Status = normalized;
    }

    /// <summary>Assign the bicycle to a bike point (IN_USE → AVAILABLE).</summary>
    /// <param name="bikePointId">Destination bike-point id.</param>
    public void AssignToBikePoint(int bikePointId)
    {
        if (CurrentBikePointId == bikePointId) return;
        CurrentBikePointId = bikePointId;
        if (Status == Statuses.InUse) Status = Statuses.Available;
    }

    /// <summary>Remove the bicycle from its current bike point (if any).</summary>
    public void RemoveFromBikePoint()
    {
        CurrentBikePointId = null;
    }

    /// <summary>Applies a partial update (edit form).</summary>
    /// <param name="command">The update command.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    public void ApplyUpdate(UpdateBicycleCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (!string.IsNullOrWhiteSpace(command.SerialNumber)) SerialNumber = command.SerialNumber.Trim();
        if (!string.IsNullOrWhiteSpace(command.BikeCode)) BikeCode = command.BikeCode.Trim();
        if (!string.IsNullOrWhiteSpace(command.QrCode)) QrCode = command.QrCode.Trim();
        if (!string.IsNullOrWhiteSpace(command.Model)) Model = command.Model.Trim();
        if (!string.IsNullOrWhiteSpace(command.Status)) UpdateStatus(command.Status);
        if (command.CurrentBikePointId.HasValue) CurrentBikePointId = command.CurrentBikePointId;
    }
}
