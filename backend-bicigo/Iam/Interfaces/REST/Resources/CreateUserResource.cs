using System.ComponentModel.DataAnnotations;

namespace backend_bicigo.Iam.Interfaces.REST.Resources;

/// <summary>
///     Request payload for POST /users (register).
/// </summary>
public sealed class CreateUserResource
{
    /// <summary>User e-mail.</summary>
    [Required(ErrorMessage = "RequiredEmail")]
    [EmailAddress(ErrorMessage = "InvalidEmail")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Plain-text password (mock).</summary>
    [Required(ErrorMessage = "RequiredPassword")]
    public string Password { get; set; } = string.Empty;

    /// <summary>Display name.</summary>
    [Required(ErrorMessage = "RequiredUsername")]
    public string Username { get; set; } = string.Empty;

    /// <summary>Phone number.</summary>
    public string? PhoneNumber { get; set; }

    /// <summary>Plan key tarifa|pro.</summary>
    public string? Plan { get; set; }

    /// <summary>Location.</summary>
    public string? Location { get; set; }
}