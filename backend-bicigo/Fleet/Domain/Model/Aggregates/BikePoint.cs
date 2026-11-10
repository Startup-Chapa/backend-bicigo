using backend_bicigo.Fleet.Domain.Model.Commands;
using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Fleet.Domain.Model.Aggregates;

/// <summary>
///     Bike-point (station) aggregate. Mirrors
///     <c>fleet-station-managment/domain/model/bikePoint.entity.js</c> and <c>db.json/bike-points</c>.
/// </summary>
public class BikePoint : IAuditableEntity
{
    /// <summary>Valid bike-point statuses (OPERATIONAL|FULL|MAINTENANCE|DISABLED).</summary>
    public static class Statuses
    {
        /// <summary>Open for docking.</summary>
        public const string Operational = "OPERATIONAL";
        /// <summary>At capacity.</summary>
        public const string Full = "FULL";
        /// <summary>Under maintenance.</summary>
        public const string Maintenance = "MAINTENANCE";
        /// <summary>Disabled.</summary>
        public const string Disabled = "DISABLED";
    }

    /// <summary>Protected parameterless constructor for EF Core.</summary>
    protected BikePoint()
    {
        Name = null!;
        District = null!;
        Address = null!;
        Status = Statuses.Operational;
    }

    /// <summary>Creates a bike point from a register command.</summary>
    /// <param name="command">The registration command.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when required fields are empty or capacity is invalid.</exception>
    public BikePoint(RegisterBikePointCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (string.IsNullOrWhiteSpace(command.Name)) throw new ArgumentException("Name is required.", nameof(command));
        if (string.IsNullOrWhiteSpace(command.District)) throw new ArgumentException("District is required.", nameof(command));
        if (string.IsNullOrWhiteSpace(command.Address)) throw new ArgumentException("Address is required.", nameof(command));
        if (command.Capacity < 1) throw new ArgumentException("Capacity must be &gt;= 1.", nameof(command));
        Name = command.Name.Trim();
        District = command.District.Trim();
        Address = command.Address.Trim();
        Latitude = command.Latitude;
        Longitude = command.Longitude;
        Capacity = command.Capacity;
        CurrentBicyclesCount = 0;
        Status = Statuses.Operational;
    }

    /// <summary>Server-generated id.</summary>
    public int Id { get; private set; }

    /// <summary>Station name.</summary>
    public string Name { get; private set; }

    /// <summary>District.</summary>
    public string District { get; private set; }

    /// <summary>Address.</summary>
    public string Address { get; private set; }

    /// <summary>Latitude.</summary>
    public double Latitude { get; private set; }

    /// <summary>Longitude.</summary>
    public double Longitude { get; private set; }

    /// <summary>Total capacity (must be &gt;= current count).</summary>
    public int Capacity { get; private set; }

    /// <summary>Free slots (calculated = Capacity - CurrentBicyclesCount).</summary>
    public int AvailableSlots => Capacity - CurrentBicyclesCount;

    /// <summary>Number of docked bicycles.</summary>
    public int CurrentBicyclesCount { get; private set; }

    /// <summary>Status OPERATIONAL|FULL|MAINTENANCE|DISABLED.</summary>
    public string Status { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>Update the status.</summary>
    /// <param name="status">New status.</param>
    /// <exception cref="ArgumentException">Thrown when status is invalid.</exception>
    public void UpdateStatus(string status)
    {
        var normalized = (status ?? string.Empty).Trim().ToUpperInvariant();
        if (normalized is not (Statuses.Operational or Statuses.Full or Statuses.Maintenance or Statuses.Disabled))
            throw new ArgumentException($"Invalid bike-point status '{status}'.", nameof(status));
        Status = normalized;
    }

    /// <summary>Assign (increase) capacity. Cannot be lower than the current docked count.</summary>
    /// <param name="capacity">New capacity.</param>
    /// <exception cref="ArgumentException">Thrown when capacity &lt; 1 or &lt; current count.</exception>
    public void AssignCapacity(int capacity)
    {
        if (capacity < 1) throw new ArgumentException("Capacity must be &gt;= 1.", nameof(capacity));
        if (capacity < CurrentBicyclesCount) throw new ArgumentException("Capacity cannot be lower than docked bicycles.", nameof(capacity));
        Capacity = capacity;
        if (AvailableSlots == 0 && Status == Statuses.Operational) Status = Statuses.Full;
        else if (AvailableSlots > 0 && Status == Statuses.Full) Status = Statuses.Operational;
    }

    /// <summary>Disable the station.</summary>
    public void Disable() => Status = Statuses.Disabled;

    /// <summary>Add a docked bicycle. Fails when full or disabled.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the station cannot accept bikes.</exception>
    public void AddBicycle()
    {
        if (Status is Statuses.Disabled or Statuses.Maintenance)
            throw new InvalidOperationException("Bike point is not operational.");
        if (AvailableSlots <= 0) throw new InvalidOperationException("Bike point is full.");
        CurrentBicyclesCount++;
        if (AvailableSlots == 0) Status = Statuses.Full;
    }

    /// <summary>Remove a docked bicycle. Opens the station again when it was FULL.</summary>
    public void RemoveBicycle()
    {
        if (CurrentBicyclesCount > 0) CurrentBicyclesCount--;
        if (Status == Statuses.Full) Status = Statuses.Operational;
    }

    /// <summary>Applies a partial update (edit form).</summary>
    /// <param name="command">The update command.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    public void ApplyUpdate(UpdateBikePointCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (!string.IsNullOrWhiteSpace(command.Name)) Name = command.Name.Trim();
        if (!string.IsNullOrWhiteSpace(command.District)) District = command.District.Trim();
        if (!string.IsNullOrWhiteSpace(command.Address)) Address = command.Address.Trim();
        if (command.Latitude.HasValue) Latitude = command.Latitude.Value;
        if (command.Longitude.HasValue) Longitude = command.Longitude.Value;
        if (command.Capacity.HasValue) AssignCapacity(command.Capacity.Value);
        if (!string.IsNullOrWhiteSpace(command.Status)) UpdateStatus(command.Status);
    }
}
