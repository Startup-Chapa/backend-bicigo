using System.ComponentModel.DataAnnotations;
using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Trip.Domain.Model.Aggregates;
using backend_bicigo.Trip.Domain.Model.Commands;
using backend_bicigo.Trip.Interfaces.REST.Resources;
using backend_bicigo.Trip.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Trip.Interfaces.REST;

/// <summary>REST endpoints for trip lifecycle, history and detail.</summary>
[ApiController]
[Route("api/v1/trips")]
[Produces("application/json")]
public class TripsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<TripsController> _logger;

    /// <summary>Initializes a new instance.</summary>
    public TripsController(AppDbContext context, ILogger<TripsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>Gets all trips, optionally filtered by user, bicycle or status.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TripResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrips(
        [FromQuery] string? userId,
        [FromQuery] string? bicycleId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var query = _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(userId)) query = query.Where(t => t.UserId == userId.Trim());
        if (!string.IsNullOrWhiteSpace(bicycleId)) query = query.Where(t => t.BicycleId == bicycleId.Trim());
        if (!string.IsNullOrWhiteSpace(status))
        {
            try
            {
                var normalizedStatus = TripAssembler.ToTripStatus(status);
                query = query.Where(t => t.Status == normalizedStatus);
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid trip status. Allowed values: PENDING, IN_PROGRESS, FINISHED.");
            }
        }

        var list = await query.OrderByDescending(t => t.CreatedAt).ToListAsync(cancellationToken);
        return Ok(list.Select(TripAssembler.ToTripFromEntity));
    }

    /// <summary>Gets a trip by its database identifier.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TripResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTripById(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>()
            .AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (entity is null) return NotFound();
        return Ok(TripAssembler.ToTripFromEntity(entity));
    }

    /// <summary>Gets the in-progress trip for a user. Returns 204 when none exists.</summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(TripResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> GetActiveTrip([FromQuery, Required] string userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId)) return BadRequest("userId is required.");
        var entity = await _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.UserId == userId.Trim() && t.Status == backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.InProgress, cancellationToken);
        return entity is null ? NoContent() : Ok(TripAssembler.ToTripFromEntity(entity));
    }

    /// <summary>Gets the completed trip history for a user.</summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<TripResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTripHistory([FromQuery, Required] string userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId)) return BadRequest("userId is required.");
        var trips = await _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>()
            .AsNoTracking()
            .Where(t => t.UserId == userId.Trim() && t.Status == backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.Finished)
            .OrderByDescending(t => t.FinishedAt)
            .ToListAsync(cancellationToken);
        return Ok(trips.Select(TripAssembler.ToTripFromEntity));
    }

    /// <summary>Creates a pending trip after checking the user and bicycle have no conflicting rental.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TripResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateTrip([FromBody] CreateTripResource resource, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(resource.UserId) || string.IsNullOrWhiteSpace(resource.BicycleId) || string.IsNullOrWhiteSpace(resource.TripId))
            return BadRequest("TripId, UserId and BicycleId are required.");

        var tripSet = _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>();
        var businessTripId = resource.TripId.Trim();
        var userId = resource.UserId.Trim();
        var bicycleId = resource.BicycleId.Trim();

        if (await tripSet.AnyAsync(t => t.TripId == businessTripId, cancellationToken))
            return Conflict("Trip id already exists.");

        var activeTripExists = await tripSet.AnyAsync(
            t => t.UserId == userId && (t.Status == backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.Pending || t.Status == backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.InProgress),
            cancellationToken);
        if (activeTripExists) return Conflict("The user already has an active trip.");

        var bicycle = await FindBicycleAsync(bicycleId, cancellationToken);
        if (bicycle is null) return NotFound("Bicycle not found.");
        if (bicycle.Status != Bicycle.Statuses.Available) return Conflict("The bicycle is not available.");

        try
        {
            var entity = new backend_bicigo.Trip.Domain.Model.Aggregates.Trip(
                new CreateTripCommand(businessTripId, userId, bicycleId, resource.StartedAt));
            await tripSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return CreatedAtAction(nameof(GetTripById), new { id = entity.Id }, TripAssembler.ToTripFromEntity(entity));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid trip creation payload");
            return BadRequest("Invalid trip fields.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Trip creation violated a persistence constraint");
            return Conflict("The trip could not be created because of a conflicting record.");
        }
    }

    /// <summary>Validates the scanned QR code and starts the trip.</summary>
    [HttpPost("{id:int}/unlock")]
    [ProducesResponseType(typeof(TripResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UnlockTrip(int id, [FromBody] UnlockTripResource resource, CancellationToken cancellationToken)
    {
        var trip = await _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (trip is null) return NotFound("Trip not found.");
        if (!string.Equals(trip.UserId, resource.UserId?.Trim(), StringComparison.Ordinal)) return NotFound("Trip not found for this user.");
        if (trip.Status == backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.Finished)
            return Conflict("A finished trip cannot be unlocked.");

        var bicycle = await FindBicycleAsync(trip.BicycleId, cancellationToken);
        if (bicycle is null) return NotFound("The bicycle associated with this trip was not found.");

        var qrValidation = new QrValidation(new CreateQrValidationCommand(
            $"QRV-{Guid.NewGuid():N}", trip.TripId, trip.BicycleId, resource.QrCodeSnapshot));
        qrValidation.Validate(bicycle.QrCode);
        await _context.Set<QrValidation>().AddAsync(qrValidation, cancellationToken);

        if (qrValidation.Result != QrValidation.Results.Valid)
        {
            await _context.SaveChangesAsync(cancellationToken);
            return BadRequest(new { message = "The QR code is invalid for this bicycle.", validation = TripAssembler.ToQrFromEntity(qrValidation) });
        }

        if (trip.Status == backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.Pending)
        {
            if (bicycle.Status != Bicycle.Statuses.Available || !bicycle.Unlock())
                return Conflict("The bicycle is no longer available.");
            trip.StartTrip();
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Ok(TripAssembler.ToTripFromEntity(trip));
    }

    /// <summary>Finishes an in-progress trip, calculates its cost and makes the bicycle available again.</summary>
    [HttpPost("{id:int}/finish")]
    [ProducesResponseType(typeof(TripResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> FinishTrip(int id, [FromBody] FinishTripResource resource, CancellationToken cancellationToken)
    {
        var trip = await _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (trip is null) return NotFound("Trip not found.");
        if (!string.Equals(trip.UserId, resource.UserId?.Trim(), StringComparison.Ordinal)) return NotFound("Trip not found for this user.");
        if (trip.Status == backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.Finished)
            return Conflict("The trip has already been finished.");
        if (trip.Status != backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.InProgress)
            return Conflict("Only an in-progress trip can be finished.");

        var bicycle = await FindBicycleAsync(trip.BicycleId, cancellationToken);
        if (bicycle is null) return NotFound("The bicycle associated with this trip was not found.");

        try
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            trip.FinishTrip(resource.DistanceKm, resource.PricePerKm, resource.IsPremium, resource.FinishedAt);
            // Do not overwrite MAINTENANCE or DISABLED if an operator changed the bike while it was rented.
            if (bicycle.Status == Bicycle.Statuses.InUse)
                bicycle.UpdateStatus(Bicycle.Statuses.Available);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return Ok(TripAssembler.ToTripFromEntity(trip));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid trip finish payload");
            return BadRequest("Distance and price must be zero or greater.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid trip state transition");
            return Conflict(ex.Message);
        }
    }

    /// <summary>Updates a trip's supported lifecycle fields. Prefer the unlock and finish actions for state transitions.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TripResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTrip(int id, [FromBody] TripResource resource, CancellationToken cancellationToken)
    {
        var entity = await _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (entity is null) return NotFound();
        if (!string.IsNullOrWhiteSpace(resource.UserId) && !string.Equals(entity.UserId, resource.UserId.Trim(), StringComparison.Ordinal))
            return NotFound("Trip not found for this user.");

        if (!string.IsNullOrWhiteSpace(resource.Status))
        {
            try
            {
                var requestedStatus = TripAssembler.ToTripStatus(resource.Status);
                if (requestedStatus == backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.InProgress)
                    entity.StartTrip(resource.StartedAt);
                else if (requestedStatus != entity.Status)
                    return BadRequest("Use POST /api/v1/trips/{id}/unlock or /finish to change the trip lifecycle.");
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                _logger.LogWarning(ex, "Invalid trip update");
                return BadRequest("Invalid trip update.");
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Ok(TripAssembler.ToTripFromEntity(entity));
    }

    /// <summary>Deletes a trip only when it has not started.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteTrip(int id, CancellationToken cancellationToken)
    {
        var entity = await _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (entity is null) return NotFound();
        if (entity.Status != backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.Pending)
            return Conflict("Only pending trips can be deleted.");
        _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private async Task<Bicycle?> FindBicycleAsync(string bicycleId, CancellationToken cancellationToken)
    {
        var bicycles = _context.Set<Bicycle>();
        var bicycle = await bicycles.FirstOrDefaultAsync(
            b => b.BikeCode == bicycleId || b.SerialNumber == bicycleId,
            cancellationToken);
        if (bicycle is not null) return bicycle;
        return int.TryParse(bicycleId, out var numericId)
            ? await bicycles.FirstOrDefaultAsync(b => b.Id == numericId, cancellationToken)
            : null;
    }
}
