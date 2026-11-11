namespace backend_bicigo.Iam.Domain.Model.Commands;

/// <summary>
///     Command for a partial user update (PATCH /users/:id), used by updateProfile.
/// </summary>
public record UpdateUserCommand(
    string? Email = null,
    string? Password = null,
    string? Username = null,
    string? PhoneNumber = null,
    string? Plan = null,
    decimal? Balance = null,
    string? Role = null,
    string? Location = null);