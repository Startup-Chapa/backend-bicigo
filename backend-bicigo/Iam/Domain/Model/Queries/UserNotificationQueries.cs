namespace backend_bicigo.Iam.Domain.Model.Queries;

/// <summary>Query to fetch a user notification by id.</summary>
/// <param name="Id">Notification id.</param>
public record GetUserNotificationByIdQuery(int Id);