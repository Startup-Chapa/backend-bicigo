using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Trip.Domain.Model.Aggregates;

/// <summary>
///     Trip user (db.json/tripUsers). Keeps the riders context separate from IAM Users.
/// </summary>
public class TripUser : IAuditableEntity
{
    /// <summary>Protected constructor for EF Core.</summary>
    protected TripUser()
    {
        UserId = null!;
        Name = null!;
        Email = null!;
        Role = "USER";
    }

    /// <summary>Creates a trip user.</summary>
    public TripUser(string userId, string name, string email, string? role = null, bool active = true)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("UserId is required.", nameof(userId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));
        UserId = userId.Trim();
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        Role = string.IsNullOrWhiteSpace(role) ? "USER" : role.Trim().ToUpperInvariant();
        Active = active;
    }

    /// <summary>Server id.</summary>
    public int Id { get; private set; }

    /// <summary>User reference (string, matches db.json USER-001).</summary>
    public string UserId { get; private set; }

    /// <summary>Display name.</summary>
    public string Name { get; private set; }

    /// <summary>E-mail.</summary>
    public string Email { get; private set; }

    /// <summary>Role USER|OPERATOR|ADMIN.</summary>
    public string Role { get; private set; }

    /// <summary>Whether the rider is active.</summary>
    public bool Active { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}
