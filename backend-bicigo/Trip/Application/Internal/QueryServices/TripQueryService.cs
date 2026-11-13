using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Trip.Application.Services;
using backend_bicigo.Trip.Domain.Model.Aggregates;
using backend_bicigo.Trip.Domain.Model.Queries;

namespace backend_bicigo.Trip.Application.Internal.QueryServices;

/// <summary>
///     Trip query service.
/// </summary>
/// <param name="context">EF Core context.</param>
public class TripQueryService(AppDbContext context) : ITripQueryService
{
    /// <inheritdoc />
    public async Task<backend_bicigo.Trip.Domain.Model.Aggregates.Trip?> Handle(GetTripByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().FindAsync([query.Id], cancellationToken);
    }

    /// <inheritdoc />
    public async Task<QrValidation?> Handle(GetQrValidationByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await context.Set<QrValidation>().FindAsync([query.Id], cancellationToken);
    }
}
