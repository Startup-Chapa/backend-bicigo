namespace backend_bicigo.Billing.Domain.Model.Commands;

/// <summary>Command to create a plan feature.</summary>
public record CreatePlanFeatureCommand(int PlanId, string Name);
