using backend_bicigo.Iam.Application.Errors;
using backend_bicigo.Iam.Domain.Model.Aggregates;
using backend_bicigo.Iam.Domain.Model.Commands;
using backend_bicigo.Shared.Application.Patterns;

namespace backend_bicigo.Iam.Application.Services;

/// <summary>
///     Command service contract for User creation.
/// </summary>
public interface IUserCommandService
{
    /// <summary>Handles the registration command.</summary>
    /// <param name="command">The registration command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created user or the error reason.</returns>
    Task<Result<User, CreateUserError>> Handle(RegisterUserCommand command, CancellationToken cancellationToken = default);
}