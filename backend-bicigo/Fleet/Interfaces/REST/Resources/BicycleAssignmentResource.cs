namespace backend_bicigo.Fleet.Interfaces.REST.Resources;

/// <summary>Bicycle assignment response.</summary>
public sealed class BicycleAssignmentResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>Bicycle id.</summary>
    public int BicycleId { get; set; }

    /// <summary>Bike-point id.</summary>
    public int BikePointId { get; set; }

    /// <summary>Action ASSIGN|UNLOCK|REMOVE.</summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>Assigned timestamp ISO.</summary>
    public string AssignedAt { get; set; } = string.Empty;
}
