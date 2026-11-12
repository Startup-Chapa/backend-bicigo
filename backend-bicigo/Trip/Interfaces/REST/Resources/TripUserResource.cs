namespace backend_bicigo.Trip.Interfaces.REST.Resources;

/// <summary>Trip user response (db.json/tripUsers).</summary>
public sealed class TripUserResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>User reference.</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>Display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>E-mail.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Role.</summary>
    public string Role { get; set; } = "USER";

    /// <summary>Whether active.</summary>
    public bool Active { get; set; } = true;
}
