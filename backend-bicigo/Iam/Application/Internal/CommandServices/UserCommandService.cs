using backend_bicigo.Iam.Application.Errors;
using backend_bicigo.Iam.Application.Services;
using backend_bicigo.Iam.Domain.Model.Aggregates;
using backend_bicigo.Iam.Domain.Model.Commands;
using backend_bicigo.Iam.Domain.Repositories;
using backend_bicigo.Shared.Application.Patterns;
using backend_bicigo.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Iam.Application.Internal.CommandServices;

/// <summary>
///     Application service handling user registration with duplicate-e-mail detection.
/// </summary>
/// <param name="userRepository">Repository for user data.</param>
/// <param name="unitOfWork">Unit of work for transaction scope.</param>
/// <param name="logger">Logger for diagnostics.</param>
public class UserCommandService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<UserCommandService> logger) : IUserCommandService
{
    /// <inheritdoc />
    public async Task<Result<User, CreateUserError>> Handle(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var existing = await userRepository.FindByEmailFirstAsync(command.Email, cancellationToken);
        if (existing is not null)
        {
            logger.LogWarning("Duplicate user e-mail rejected: {Email}", command.Email);
            return new Result<User, CreateUserError>.Failure(CreateUserError.DuplicateEmail);
        }

        try
        {
            var user = new User(command);
            await userRepository.AddAsync(user, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<User, CreateUserError>.Success(user);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid arguments while creating user: {Email}", command.Email);
            return new Result<User, CreateUserError>.Failure(CreateUserError.InvalidEmail);
        }
        catch (DbUpdateException ex) when (IsDuplicateKeyViolation(ex))
        {
            logger.LogWarning(ex, "Duplicate key violation creating user: {Email}", command.Email);
            return new Result<User, CreateUserError>.Failure(CreateUserError.DuplicateEmail);
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database update failed creating user: {Email}", command.Email);
            return new Result<User, CreateUserError>.Failure(CreateUserError.UnexpectedError);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating user: {Email}", command.Email);
            return new Result<User, CreateUserError>.Failure(CreateUserError.UnexpectedError);
        }
    }

    private static bool IsDuplicateKeyViolation(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (!string.Equals(current.GetType().Name, "MySqlException", StringComparison.Ordinal)) continue;
            var numberProperty = current.GetType().GetProperty("Number");
            if (numberProperty?.PropertyType == typeof(int)
                && numberProperty.GetValue(current) is int errorCode
                && errorCode == 1062) return true;
        }
        return false;
    }
}
