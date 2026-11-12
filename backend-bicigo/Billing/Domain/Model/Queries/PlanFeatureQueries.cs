namespace backend_bicigo.Billing.Domain.Model.Queries;

/// <summary>Query to fetch a plan feature by id.</summary>
/// <param name="Id">Feature id.</param>
public record GetPlanFeatureByIdQuery(int Id);
