using backend_bicigo.Iam.Domain.Model.Aggregates;
using backend_bicigo.Iam.Domain.Repositories;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Iam.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     EF Core repository for User persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class UserRepository(AppDbContext context)
    : BaseRepository<User>(context), IUserRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<User>> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>()
            .Where(u => u.Email.ToLower() == email.Trim().ToLower())
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User?> FindByEmailFirstAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.Trim().ToLower(), cancellationToken);
    }
}