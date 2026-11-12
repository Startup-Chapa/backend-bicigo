using System.Text.Json.Serialization;

namespace backend_bicigo.Iam.Interfaces.REST.Resources;

/// <summary>
///     PATCH payload for /users/:id (updateProfile).
/// </summary>
public sealed class UpdateUserResource
{
    /// <summary>E-mail.</summary>
    public string? Email { get; set; }

    /// <summary>Password.</summary>
    public string? Password { get; set; }

    /// <summary>Display name.</summary>
    public string? Username { get; set; }

    /// <summary>Phone number.</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>Plan key.</summary>
    public string? Plan { get; set; }

    /// <summary>Balance.</summary>
    public decimal? Balance { get; set; }

    /// <summary>Role.</summary>
    public string? Role { get; set; }

    /// <summary>Location.</summary>
    public string? Location { get; set; }
}