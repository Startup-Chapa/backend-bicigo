namespace backend_bicigo.Billing.Domain.Model.Queries;

/// <summary>Query to fetch a plan by id (GET /plans/:id).</summary>
/// <param name="Id">Plan id.</param>
public record GetPlanByIdQuery(int Id);

/// <summary>Query to fetch subscriptions by user (GET /subscriptions?userId=).</summary>
/// <param name="UserId">User id.</param>
public record GetSubscriptionsByUserQuery(int UserId);

/// <summary>Query to fetch payments by user (GET /payments?userId=).</summary>
/// <param name="UserId">User id.</param>
public record GetPaymentsByUserQuery(int UserId);

/// <summary>Query to fetch payment methods by user (GET /payment-methods?userId=).</summary>
/// <param name="UserId">User id.</param>
public record GetPaymentMethodsByUserQuery(int UserId);
