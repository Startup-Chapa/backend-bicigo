using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace backend_bicigo.Fleet.Infrastructure.Persistence.EFC.Repositories;

/// <summary>
///     EF Core repository for BikePoint persistence.
/// </summary>
/// <param name="context">The EF Core database context.</param>
public class BikePointRepository(AppDbContext context) : BaseRepository<BikePoint>(context)
{
}
