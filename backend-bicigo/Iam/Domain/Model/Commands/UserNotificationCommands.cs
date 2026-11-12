namespace backend_bicigo.Iam.Domain.Model.Commands;

/// <summary>Command to create a user notification.</summary>
public record CreateUserNotificationCommand(int UserId, string Title, string Message, string? Type = null);