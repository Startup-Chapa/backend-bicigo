using backend_bicigo.Iam.Domain.Model.Aggregates;
using backend_bicigo.Iam.Domain.Model.Queries;

namespace backend_bicigo.Iam.Application.Services;

/// <summary>
///     Query service contract for User reads.
/// </summary>
public interface IUserQueryService
{
    /// <summary>Handles a get-by-id query.</summary>
    Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Handles a get-by-email query (returns all matches for mock login filter).</summary>
    Task<IEnumerable<User>> Handle(GetUsersByEmailQuery query, CancellationToken cancellationToken = default);
}