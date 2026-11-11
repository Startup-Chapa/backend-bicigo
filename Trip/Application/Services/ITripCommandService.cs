using backend_bicigo.Shared.Application.Patterns;
using backend_bicigo.Trip.Application.Errors;
using backend_bicigo.Trip.Domain.Model.Aggregates;
using backend_bicigo.Trip.Domain.Model.Commands;

namespace backend_bicigo.Trip.Application.Services;

/// <summary>Command service contract for Trip writes.</summary>
public interface ITripCommandService
{
    /// <summary>Handles trip creation.</summary>
    Task<Result<backend_bicigo.Trip.Domain.Model.Aggregates.Trip, CreateTripError>> Handle(CreateTripCommand command, CancellationToken cancellationToken = default);

    /// <summary>Handles QR validation creation.</summary>
    Task<Result<QrValidation, CreateTripError>> Handle(CreateQrValidationCommand command, CancellationToken cancellationToken = default);
}
