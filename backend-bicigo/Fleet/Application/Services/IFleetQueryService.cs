using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Fleet.Domain.Model.Queries;

namespace backend_bicigo.Fleet.Application.Services;

/// <summary>Query service contract for Fleet reads.</summary>
public interface IFleetQueryService
{
    /// <summary>Handles a get-bicycle-by-id query.</summary>
    Task<Bicycle?> Handle(GetBicycleByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Handles a get-bike-point-by-id query.</summary>
    Task<BikePoint?> Handle(GetBikePointByIdQuery query, CancellationToken cancellationToken = default);
}
