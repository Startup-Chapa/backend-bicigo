using backend_bicigo.Fleet.Application.Errors;
using backend_bicigo.Fleet.Application.Services;
using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Fleet.Domain.Model.Commands;
using backend_bicigo.Shared.Application.Patterns;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Fleet.Application.Internal.CommandServices;

/// <summary>
///     Fleet command service: registers bicycles and bike points keeping dock counts consistent.
/// </summary>
/// <param name="context">EF Core context.</param>
public class FleetCommandService(AppDbContext context) : IFleetCommandService
{
    /// <inheritdoc />
    public async Task<Result<Bicycle, CreateFleetError>> Handle(RegisterBicycleCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var duplicate = await context.Set<Bicycle>()
                .AnyAsync(b => b.BikeCode == command.BikeCode.Trim() || b.SerialNumber == command.SerialNumber.Trim(), cancellationToken);
            if (duplicate) return new Result<Bicycle, CreateFleetError>.Failure(CreateFleetError.DuplicateBikeCode);

            var entity = new Bicycle(command);
            await context.Set<Bicycle>().AddAsync(entity, cancellationToken);
            if (command.CurrentBikePointId.HasValue)
            {
                var point = await context.Set<BikePoint>().FindAsync([command.CurrentBikePointId.Value], cancellationToken);
                point?.AddBicycle();
            }
            await context.SaveChangesAsync(cancellationToken);
            return new Result<Bicycle, CreateFleetError>.Success(entity);
        }
        catch (ArgumentException)
        {
            return new Result<Bicycle, CreateFleetError>.Failure(CreateFleetError.InvalidInput);
        }
        catch (InvalidOperationException)
        {
            return new Result<Bicycle, CreateFleetError>.Failure(CreateFleetError.InvalidInput);
        }
        catch (Exception)
        {
            return new Result<Bicycle, CreateFleetError>.Failure(CreateFleetError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<BikePoint, CreateFleetError>> Handle(RegisterBikePointCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = new BikePoint(command);
            await context.Set<BikePoint>().AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return new Result<BikePoint, CreateFleetError>.Success(entity);
        }
        catch (ArgumentException)
        {
            return new Result<BikePoint, CreateFleetError>.Failure(CreateFleetError.InvalidInput);
        }
        catch (Exception)
        {
            return new Result<BikePoint, CreateFleetError>.Failure(CreateFleetError.UnexpectedError);
        }
    }
}
