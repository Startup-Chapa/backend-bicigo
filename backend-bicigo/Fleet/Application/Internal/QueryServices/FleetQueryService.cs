using backend_bicigo.Fleet.Application.Services;
using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Fleet.Domain.Model.Queries;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace backend_bicigo.Fleet.Application.Internal.QueryServices;

/// <summary>
///     Fleet query service.
/// </summary>
/// <param name="context">EF Core context.</param>
public class FleetQueryService(AppDbContext context) : IFleetQueryService
{
    /// <inheritdoc />
    public async Task<Bicycle?> Handle(GetBicycleByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await context.Set<Bicycle>().FindAsync([query.Id], cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BikePoint?> Handle(GetBikePointByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await context.Set<BikePoint>().FindAsync([query.Id], cancellationToken);
    }
}
