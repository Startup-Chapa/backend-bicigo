using backend_bicigo.Fleet.Application.Errors;
using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Fleet.Domain.Model.Commands;
using backend_bicigo.Fleet.Domain.Model.Queries;
using backend_bicigo.Shared.Application.Patterns;

namespace backend_bicigo.Fleet.Application.Services;

/// <summary>Command service contract for Fleet writes.</summary>
public interface IFleetCommandService
{
    /// <summary>Handles bicycle registration.</summary>
    Task<Result<Bicycle, CreateFleetError>> Handle(RegisterBicycleCommand command, CancellationToken cancellationToken = default);

    /// <summary>Handles bike-point registration.</summary>
    Task<Result<BikePoint, CreateFleetError>> Handle(RegisterBikePointCommand command, CancellationToken cancellationToken = default);
}
