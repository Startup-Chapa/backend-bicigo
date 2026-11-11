using backend_bicigo.Trip.Domain.Model.Commands;
using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Trip.Domain.Model.Aggregates;

/// <summary>
///     Trip aggregate. Mirrors <c>trip-management/domain/model/trip.entity.js</c> and <c>db.json/trips</c>.
/// </summary>
public class Trip : IAuditableEntity
{
    /// <summary>Valid trip statuses (PENDING|IN_PROGRESS|FINISHED).</summary>
    public static class Statuses
    {
        /// <summary>Created, not started.</summary>
        public const string Pending = "PENDING";
        /// <summary>Currently ridden.</summary>
        public const string InProgress = "IN_PROGRESS";
        /// <summary>Completed.</summary>
        public const string Finished = "FINISHED";
    }

    /// <summary>Protected parameterless constructor for EF Core.</summary>
    protected Trip()
    {
        TripId = null!;
        UserId = null!;
        BicycleId = null!;
        Status = Statuses.Pending;
    }

    /// <summary>Creates a trip from a command.</summary>
    /// <param name="command">The creation command.</param>
    /// <exception cref="ArgumentNullException">Thrown when command is null.</exception>
    /// <exception cref="ArgumentException">Thrown when required fields are empty.</exception>
    public Trip(CreateTripCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (string.IsNullOrWhiteSpace(command.TripId)) throw new ArgumentException("TripId is required.", nameof(command));
        if (string.IsNullOrWhiteSpace(command.UserId)) throw new ArgumentException("UserId is required.", nameof(command));
        if (string.IsNullOrWhiteSpace(command.BicycleId)) throw new ArgumentException("BicycleId is required.", nameof(command));
        TripId = command.TripId.Trim();
        UserId = command.UserId.Trim();
        BicycleId = command.BicycleId.Trim();
        StartedAt = command.StartedAt;
        FinishedAt = null;
        DistanceKm = 0m;
        Status = Statuses.Pending;
        TotalCost = 0m;
    }

    /// <summary>Server-generated id (numeric for CRUD).</summary>
    public int Id { get; private set; }

    /// <summary>Business trip id, e.g. TRIP-001.</summary>
    public string TripId { get; private set; }

    /// <summary>User reference (string to match db.json USER-001 shape).</summary>
    public string UserId { get; private set; }

    /// <summary>Bicycle reference (string to match db.json BIKE-001 shape).</summary>
    public string BicycleId { get; private set; }

    /// <summary>Started timestamp ISO (nullable until started).</summary>
    public string? StartedAt { get; private set; }

    /// <summary>Finished timestamp ISO (nullable).</summary>
    public string? FinishedAt { get; private set; }

    /// <summary>Distance in km.</summary>
    public decimal DistanceKm { get; private set; }

    /// <summary>Status PENDING|IN_PROGRESS|FINISHED.</summary>
    public string Status { get; private set; }

    /// <summary>Total cost in PEN.</summary>
    public decimal TotalCost { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>Start the trip (idempotent when already IN_PROGRESS; fails when FINISHED).</summary>
    /// <param name="startedAt">Start timestamp ISO (defaults to now).</param>
    /// <exception cref="InvalidOperationException">Thrown when the trip is FINISHED.</exception>
    public void StartTrip(string? startedAt = null)
    {
        if (Status == Statuses.Finished) throw new InvalidOperationException("Cannot start a finished trip.");
        if (Status == Statuses.InProgress) return;
        Status = Statuses.InProgress;
        StartedAt ??= startedAt ?? DateTime.UtcNow.ToString("o");
    }

    /// <summary>Finish the trip with distance and cost (only from IN_PROGRESS).</summary>
    /// <param name="distanceKm">Distance (&gt;= 0).</param>
    /// <param name="pricePerKm">Rate per km in PEN.</param>
    /// <param name="isPremium">Premium (pro) trips cost 0.</param>
    /// <param name="finishedAt">Finish timestamp ISO (optional).</param>
    /// <exception cref="ArgumentException">Thrown when distance is negative.</exception>
    /// <exception cref="InvalidOperationException">Thrown when not IN_PROGRESS.</exception>
    public void FinishTrip(decimal distanceKm, decimal pricePerKm, bool isPremium, string? finishedAt = null)
    {
        if (Status != Statuses.InProgress) throw new InvalidOperationException("Only an in-progress trip can be finished.");
        if (distanceKm < 0) throw new ArgumentException("Distance cannot be negative.", nameof(distanceKm));
        if (pricePerKm < 0) throw new ArgumentException("Price per kilometre cannot be negative.", nameof(pricePerKm));
        DistanceKm = distanceKm;
        FinishedAt = finishedAt ?? DateTime.UtcNow.ToString("o");
        TotalCost = CalculateCost(pricePerKm, isPremium);
        Status = Statuses.Finished;
    }

    /// <summary>Calculate the cost for the current distance.</summary>
    /// <param name="pricePerKm">Rate per km.</param>
    /// <param name="isPremium">Premium trips are free.</param>
    /// <returns>The calculated cost rounded to 2 decimals (0 for premium).</returns>
    public decimal CalculateCost(decimal pricePerKm, bool isPremium)
    {
        if (isPremium) return 0m;
        return Math.Round(DistanceKm * pricePerKm, 2, MidpointRounding.AwayFromZero);
    }
}
