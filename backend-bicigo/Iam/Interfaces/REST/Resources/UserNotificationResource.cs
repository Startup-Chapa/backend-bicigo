namespace backend_bicigo.Iam.Interfaces.REST.Resources;

/// <summary>User notification response.</summary>
public sealed class UserNotificationResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>User id.</summary>
    public int UserId { get; set; }

    /// <summary>Title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Message.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Type INFO|ALERT|SUCCESS.</summary>
    public string Type { get; set; } = "INFO";

    /// <summary>Whether it was read.</summary>
    public bool IsRead { get; set; }
}