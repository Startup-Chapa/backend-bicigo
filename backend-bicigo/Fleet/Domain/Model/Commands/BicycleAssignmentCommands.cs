namespace backend_bicigo.Fleet.Domain.Model.Commands;

/// <summary>Command to record a bicycle assignment.</summary>
public record CreateBicycleAssignmentCommand(int BicycleId, int BikePointId, string Action, string? AssignedAt = null);
