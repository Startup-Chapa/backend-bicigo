using backend_bicigo.Iam.Domain.Model.Commands;
using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Iam.Domain.Model.Aggregates;

/// <summary>
///     User aggregate. Mirrors <c>iam/domain/model/user.entity.js</c> and <c>db.json/users</c>.
/// </summary>
/// <remarks>
///     Fields (db.json): id, email, password, username, phoneNumber, plan (tarifa|pro), balance, role, location.
///     Password is stored in plain text only to stay compatible with the frontend mock (GET /users?email= + client filter).
/// </remarks>
public class User : IAuditableEntity
{
    /// <summary>Protected parameterless constructor for EF Core.</summary>
    protected User()
    {
        Email = null!;
        Password = null!;
        Username = null!;
        PhoneNumber = string.Empty;
        Plan = "tarifa";
        Role = "USER";
        Location = string.Empty;
    }

    /// <summary>Creates a new <see cref="User"/> from a creation command.</summary>
    /// <param name="command">The creation command.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public User(RegisterUserCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        Email = command.Email;
        Password = command.Password;
        Username = command.Username;
        PhoneNumber = command.PhoneNumber ?? string.Empty;
        Plan = command.Plan ?? "tarifa";
        Balance = 0m;
        Role = "USER";
        Location = command.Location ?? string.Empty;
    }

    /// <summary>Server-generated identifier.</summary>
    public int Id { get; private set; }

    /// <summary>User e-mail (unique).</summary>
    public string Email { get; private set; }

    /// <summary>Plain-text password (mock compatible with db.json; do not use in production).</summary>
    public string Password { get; private set; }

    /// <summary>Display name.</summary>
    public string Username { get; private set; }

    /// <summary>Phone number.</summary>
    public string PhoneNumber { get; private set; }

    /// <summary>Plan key: tarifa | pro.</summary>
    public string Plan { get; private set; }

    /// <summary>Current balance in PEN.</summary>
    public decimal Balance { get; private set; }

    /// <summary>Role: USER | OPERATOR | ADMIN.</summary>
    public string Role { get; private set; }

    /// <summary>Location/city.</summary>
    public string Location { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>Partial update used by PATCH /users/:id (updateProfile in frontend).</summary>
    /// <param name="command">The update command.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public void ApplyUpdate(UpdateUserCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (!string.IsNullOrWhiteSpace(command.Email)) Email = command.Email.Trim().ToLowerInvariant();
        if (!string.IsNullOrWhiteSpace(command.Password)) Password = command.Password;
        if (!string.IsNullOrWhiteSpace(command.Username)) Username = command.Username;
        if (command.PhoneNumber is not null) PhoneNumber = command.PhoneNumber;
        if (!string.IsNullOrWhiteSpace(command.Plan)) Plan = command.Plan;
        if (command.Balance.HasValue) Balance = command.Balance.Value;
        if (!string.IsNullOrWhiteSpace(command.Role)) Role = command.Role;
        if (command.Location is not null) Location = command.Location;
    }
}