using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Shared.Application.Patterns;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Trip.Application.Errors;
using backend_bicigo.Trip.Application.Services;
using backend_bicigo.Trip.Domain.Model.Aggregates;
using backend_bicigo.Trip.Domain.Model.Commands;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Trip.Application.Internal.CommandServices;

/// <summary>
///     Trip command service.
/// </summary>
/// <param name="context">EF Core context.</param>
public class TripCommandService(AppDbContext context) : ITripCommandService
{
    /// <inheritdoc />
    public async Task<Result<backend_bicigo.Trip.Domain.Model.Aggregates.Trip, CreateTripError>> Handle(CreateTripCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var duplicate = await context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>()
                .AnyAsync(t => t.TripId == command.TripId.Trim(), cancellationToken);
            if (duplicate) return new Result<backend_bicigo.Trip.Domain.Model.Aggregates.Trip, CreateTripError>.Failure(CreateTripError.DuplicateTripId);

            var activeTrip = await context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>()
                .AnyAsync(t => t.UserId == command.UserId.Trim() &&
                    (t.Status == backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.Pending ||
                     t.Status == backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.InProgress), cancellationToken);
            if (activeTrip) return new Result<backend_bicigo.Trip.Domain.Model.Aggregates.Trip, CreateTripError>.Failure(CreateTripError.ActiveTripExists);

            var bicycle = await FindBicycleAsync(command.BicycleId, cancellationToken);
            if (bicycle is null) return new Result<backend_bicigo.Trip.Domain.Model.Aggregates.Trip, CreateTripError>.Failure(CreateTripError.RelatedResourceNotFound);
            if (bicycle.Status != Bicycle.Statuses.Available)
                return new Result<backend_bicigo.Trip.Domain.Model.Aggregates.Trip, CreateTripError>.Failure(CreateTripError.BicycleUnavailable);

            var entity = new backend_bicigo.Trip.Domain.Model.Aggregates.Trip(command);
            await context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return new Result<backend_bicigo.Trip.Domain.Model.Aggregates.Trip, CreateTripError>.Success(entity);
        }
        catch (ArgumentException)
        {
            return new Result<backend_bicigo.Trip.Domain.Model.Aggregates.Trip, CreateTripError>.Failure(CreateTripError.InvalidInput);
        }
        catch (Exception)
        {
            return new Result<backend_bicigo.Trip.Domain.Model.Aggregates.Trip, CreateTripError>.Failure(CreateTripError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<QrValidation, CreateTripError>> Handle(CreateQrValidationCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var duplicate = await context.Set<QrValidation>().AnyAsync(q => q.ValidationId == command.ValidationId.Trim(), cancellationToken);
            if (duplicate) return new Result<QrValidation, CreateTripError>.Failure(CreateTripError.DuplicateTripId);

            var trip = await context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>()
                .FirstOrDefaultAsync(t => t.TripId == command.TripId.Trim(), cancellationToken);
            if (trip is null || trip.BicycleId != command.BicycleId.Trim())
                return new Result<QrValidation, CreateTripError>.Failure(CreateTripError.RelatedResourceNotFound);
            var bicycle = await FindBicycleAsync(command.BicycleId, cancellationToken);
            if (bicycle is null) return new Result<QrValidation, CreateTripError>.Failure(CreateTripError.RelatedResourceNotFound);

            var entity = new QrValidation(command);
            entity.Validate(bicycle.QrCode);
            await context.Set<QrValidation>().AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return new Result<QrValidation, CreateTripError>.Success(entity);
        }
        catch (ArgumentException)
        {
            return new Result<QrValidation, CreateTripError>.Failure(CreateTripError.InvalidInput);
        }
        catch (Exception)
        {
            return new Result<QrValidation, CreateTripError>.Failure(CreateTripError.UnexpectedError);
        }
    }

    private async Task<Bicycle?> FindBicycleAsync(string bicycleId, CancellationToken cancellationToken)
    {
        var bicycles = context.Set<Bicycle>();
        var bicycle = await bicycles.FirstOrDefaultAsync(
            b => b.BikeCode == bicycleId.Trim() || b.SerialNumber == bicycleId.Trim(), cancellationToken);
        if (bicycle is not null) return bicycle;
        return int.TryParse(bicycleId, out var numericId)
            ? await bicycles.FirstOrDefaultAsync(b => b.Id == numericId, cancellationToken)
            : null;
    }

}
