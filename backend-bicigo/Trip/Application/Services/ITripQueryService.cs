using backend_bicigo.Trip.Domain.Model.Aggregates;
using backend_bicigo.Trip.Domain.Model.Queries;

namespace backend_bicigo.Trip.Application.Services;

/// <summary>Query service contract for Trip reads.</summary>
public interface ITripQueryService
{
    /// <summary>Handles a get-trip-by-id query.</summary>
    Task<backend_bicigo.Trip.Domain.Model.Aggregates.Trip?> Handle(GetTripByIdQuery query, CancellationToken cancellationToken = default);

    /// <summary>Handles a get-qr-validation-by-id query.</summary>
    Task<QrValidation?> Handle(GetQrValidationByIdQuery query, CancellationToken cancellationToken = default);
}
