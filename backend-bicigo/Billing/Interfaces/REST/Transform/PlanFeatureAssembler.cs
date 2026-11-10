using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Billing.Interfaces.REST.Resources;

namespace backend_bicigo.Billing.Interfaces.REST.Transform;

/// <summary>Assembles PlanFeature resources.</summary>
public static class PlanFeatureAssembler
{
    /// <summary>Assembles a resource from an entity.</summary>
    /// <param name="entity">The feature.</param>
    /// <returns>The resource.</returns>
    public static PlanFeatureResource ToResourceFromEntity(PlanFeature entity) => new()
    {
        Id = entity.Id,
        PlanId = entity.PlanId,
        Name = entity.Name
    };
}
