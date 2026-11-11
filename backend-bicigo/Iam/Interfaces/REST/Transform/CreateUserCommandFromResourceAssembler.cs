using backend_bicigo.Iam.Domain.Model.Commands;
using backend_bicigo.Iam.Domain.Model.ValueObjects;
using backend_bicigo.Iam.Interfaces.REST.Resources;

namespace backend_bicigo.Iam.Interfaces.REST.Transform;

/// <summary>
///     Assembles a <see cref="RegisterUserCommand"/> from a <see cref="CreateUserResource"/>.
/// </summary>
public static class CreateUserCommandFromResourceAssembler
{
    /// <summary>Assembles the command from the resource.</summary>
    /// <param name="resource">The create resource.</param>
    /// <returns>The assembled command.</returns>
    public static RegisterUserCommand ToCommandFromResource(CreateUserResource resource)
    {
        _ = new Email(resource.Email); // validation (throws ArgumentException on bad e-mail)
        return new RegisterUserCommand(
            Email: resource.Email.Trim().ToLowerInvariant(),
            Password: resource.Password,
            Username: resource.Username.Trim(),
            PhoneNumber: resource.PhoneNumber?.Trim(),
            Plan: string.IsNullOrWhiteSpace(resource.Plan) ? "tarifa" : new PlanKey(resource.Plan).Value,
            Location: resource.Location?.Trim());
    }
}