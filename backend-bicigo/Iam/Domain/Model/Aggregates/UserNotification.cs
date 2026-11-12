using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Iam.Domain.Model.Aggregates;

/// <summary>
///     User notification (profile "Notificaciones" tab + TopBar bell).
/// </summary>
public class UserNotification : IAuditableEntity
{
    /// <summary>Protected constructor for EF Core.</summary>
    protected UserNotification()
    {
        Title = null!;
        Message = null!;
        Type = "INFO";
    }

    /// <summary>Creates a notification.</summary>
    public UserNotification(int userId, string title, string message, string? type = null, bool isRead = false)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("Message is required.", nameof(message));
        UserId = userId;
        Title = title.Trim();
        Message = message.Trim();
        Type = string.IsNullOrWhiteSpace(type) ? "INFO" : type.Trim().ToUpperInvariant();
        IsRead = isRead;
    }

    /// <summary>Server id.</summary>
    public int Id { get; private set; }

    /// <summary>User id.</summary>
    public int UserId { get; private set; }

    /// <summary>Title.</summary>
    public string Title { get; private set; }

    /// <summary>Message.</summary>
    public string Message { get; private set; }

    /// <summary>Type INFO|ALERT|SUCCESS.</summary>
    public string Type { get; private set; }

    /// <summary>Whether it was read.</summary>
    public bool IsRead { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>Mark as read.</summary>
    public void MarkAsRead() => IsRead = true;
}