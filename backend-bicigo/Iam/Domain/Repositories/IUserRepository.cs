using backend_bicigo.Iam.Domain.Model.Aggregates;
using backend_bicigo.Shared.Domain.Repositories;

namespace backend_bicigo.Iam.Domain.Repositories;

/// <summary>
///     The User repository contract.
/// </summary>
public interface IUserRepository : IBaseRepository<User>
{
    /// <summary>Find users by e-mail (case-insensitive).</summary>
    /// <param name="email">E-mail to search.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Matching users (empty when none).</returns>
    Task<IEnumerable<User>> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Find a user by e-mail.</summary>
    /// <param name="email">E-mail to search.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user or null.</returns>
    Task<User?> FindByEmailFirstAsync(string email, CancellationToken cancellationToken = default);
}