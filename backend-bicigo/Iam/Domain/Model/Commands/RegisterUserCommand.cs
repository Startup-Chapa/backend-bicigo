namespace backend_bicigo.Iam.Domain.Model.Commands;

/// <summary>
///     Command to register a new user (POST /users).
/// </summary>
/// <param name="Email">User e-mail (required, unique).</param>
/// <param name="Password">Plain-text password for mock compatibility.</param>
/// <param name="Username">Display name.</param>
/// <param name="PhoneNumber">Phone number (optional).</param>
/// <param name="Plan">Plan key tarifa|pro (optional, defaults tarifa).</param>
/// <param name="Location">Location (optional).</param>
public record RegisterUserCommand(
    string Email,
    string Password,
    string Username,
    string? PhoneNumber = null,
    string? Plan = null,
    string? Location = null);