using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Fleet.Interfaces.REST.Resources;

namespace backend_bicigo.Fleet.Interfaces.REST.Transform;

/// <summary>Assembles BicycleAssignment resources.</summary>
public static class BicycleAssignmentAssembler
{
    /// <summary>Assembles a resource from an entity.</summary>
    /// <param name="entity">The assignment.</param>
    /// <returns>The resource.</returns>
    public static BicycleAssignmentResource ToResourceFromEntity(BicycleAssignment entity) => new()
    {
        Id = entity.Id,
        BicycleId = entity.BicycleId,
        BikePointId = entity.BikePointId,
        Action = entity.Action,
        AssignedAt = entity.AssignedAt
    };
}
