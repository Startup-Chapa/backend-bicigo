using backend_bicigo.Iam.Application.Services;
using backend_bicigo.Iam.Domain.Model.Aggregates;
using backend_bicigo.Iam.Domain.Model.Queries;
using backend_bicigo.Iam.Domain.Repositories;

namespace backend_bicigo.Iam.Application.Internal.QueryServices;

/// <summary>
///     Query service for User reads.
/// </summary>
/// <param name="userRepository">Repository for user data.</param>
public class UserQueryService(IUserRepository userRepository) : IUserQueryService
{
    /// <inheritdoc />
    public async Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await userRepository.FindByIdAsync(query.Id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<User>> Handle(GetUsersByEmailQuery query, CancellationToken cancellationToken = default)
    {
        return await userRepository.FindByEmailAsync(query.Email, cancellationToken);
    }
}