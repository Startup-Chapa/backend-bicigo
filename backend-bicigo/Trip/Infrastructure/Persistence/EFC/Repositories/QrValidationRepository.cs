using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Repositories;
using backend_bicigo.Trip.Domain.Model.Aggregates;

namespace backend_bicigo.Trip.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     EF Core repository for QrValidation persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class QrValidationRepository(AppDbContext context) : BaseRepository<QrValidation>(context)
{
}
