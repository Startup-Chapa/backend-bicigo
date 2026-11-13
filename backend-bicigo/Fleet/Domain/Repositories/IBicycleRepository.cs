using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Shared.Domain.Repositories;

namespace backend_bicigo.Fleet.Domain.Repositories;

/// <summary>The Bicycle repository contract.</summary>
public interface IBicycleRepository : IBaseRepository<Bicycle>
{
    /// <summary>Find bicycles docked at a bike point.</summary>
    /// <param name="bikePointId">Bike-point id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Docked bicycles.</returns>
    Task<IEnumerable<Bicycle>> FindByBikePointAsync(int bikePointId, CancellationToken cancellationToken = default);

    /// <summary>Find a bicycle by its business code (e.g. BGO-1042).</summary>
    /// <param name="bikeCode">Bike code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The bicycle or null.</returns>
    Task<Bicycle?> FindByBikeCodeAsync(string bikeCode, CancellationToken cancellationToken = default);
}
