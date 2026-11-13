using backend_bicigo.Iam.Domain.Model.Aggregates;
using backend_bicigo.Iam.Interfaces.REST.Resources;

namespace backend_bicigo.Iam.Interfaces.REST.Transform;

/// <summary>
///     Assembles a <see cref="UserResource"/> from a <see cref="User"/> entity.
/// </summary>
public static class UserResourceFromEntityAssembler
{
    /// <summary>Assembles the resource from the entity.</summary>
    /// <param name="entity">The user entity.</param>
    /// <returns>The assembled resource.</returns>
    public static UserResource ToResourceFromEntity(User entity) => new()
    {
        Id = entity.Id,
        Email = entity.Email,
        Password = entity.Password,
        Username = entity.Username,
        PhoneNumber = entity.PhoneNumber,
        Plan = entity.Plan,
        Balance = entity.Balance,
        Role = entity.Role,
        Location = entity.Location
    };
}