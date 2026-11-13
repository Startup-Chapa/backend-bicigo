using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Fleet.Domain.Model.Aggregates;

/// <summary>
///     Bicycle assignment history (assign / unlock / remove / reassign).
/// </summary>
public class BicycleAssignment : IAuditableEntity
{
    /// <summary>Valid action types.</summary>
    public static class Actions
    {
        /// <summary>Assigned to a station.</summary>
        public const string Assign = "ASSIGN";
        /// <summary>Unlocked (removed from station).</summary>
        public const string Unlock = "UNLOCK";
        /// <summary>Removed from station.</summary>
        public const string Remove = "REMOVE";
    }

    /// <summary>Protected constructor for EF Core.</summary>
    protected BicycleAssignment()
    {
        Action = Actions.Assign;
        AssignedAt = string.Empty;
    }

    /// <summary>Creates an assignment record.</summary>
    public BicycleAssignment(int bicycleId, int bikePointId, string action, string? assignedAt = null)
    {
        var normalized = (action ?? string.Empty).Trim().ToUpperInvariant();
        if (normalized is not (Actions.Assign or Actions.Unlock or Actions.Remove))
            throw new ArgumentException($"Invalid action '{action}'.", nameof(action));
        BicycleId = bicycleId;
        BikePointId = bikePointId;
        Action = normalized;
        AssignedAt = assignedAt ?? DateTime.UtcNow.ToString("o");
    }

    /// <summary>Server id.</summary>
    public int Id { get; private set; }

    /// <summary>Bicycle id.</summary>
    public int BicycleId { get; private set; }

    /// <summary>Bike-point id.</summary>
    public int BikePointId { get; private set; }

    /// <summary>Action ASSIGN|UNLOCK|REMOVE.</summary>
    public string Action { get; private set; }

    /// <summary>Assignment timestamp ISO.</summary>
    public string AssignedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}
