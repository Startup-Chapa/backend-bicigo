using System.Text.Json.Serialization;

namespace backend_bicigo.Iam.Interfaces.REST.Resources;

/// <summary>
///     Response payload for User (matches db.json/users shape used by UserAssembler).
/// </summary>
public sealed class UserResource
{
    /// <summary>Server id.</summary>
    public int Id { get; set; }

    /// <summary>User e-mail.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Plain-text password (mock).</summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public string Password { get; set; } = string.Empty;

    /// <summary>Display name.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>Phone number.</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>Plan key tarifa|pro.</summary>
    public string Plan { get; set; } = "tarifa";

    /// <summary>Balance in PEN.</summary>
    public decimal Balance { get; set; }

    /// <summary>Role USER|OPERATOR|ADMIN.</summary>
    public string Role { get; set; } = "USER";

    /// <summary>Location.</summary>
    public string Location { get; set; } = string.Empty;
}