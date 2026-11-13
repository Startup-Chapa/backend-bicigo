using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Trip.Application.Errors;
using backend_bicigo.Trip.Application.Services;
using backend_bicigo.Trip.Domain.Model.Aggregates;
using backend_bicigo.Trip.Domain.Model.Commands;
using backend_bicigo.Trip.Interfaces.REST.Resources;
using backend_bicigo.Trip.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Trip.Interfaces.REST;

/// <summary>
///     Trips controller: GET /trips, POST /trips, GET /trips/{id}, PUT /trips/{id},
///     POST /trips/{id}/finish, DELETE /trips/{id}.
///     Supports GET /trips?userId= for the trip list filter (iam.currentUser.id).
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class TripsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ITripCommandService _tripCommandService;
    private readonly ILogger<TripsController> _logger;

    /// <summary>Initializes a new instance.</summary>
    public TripsController(
        AppDbContext context,
        ITripCommandService tripCommandService,
        ILogger<TripsController> logger)
    {
        _context = context;
        _tripCommandService = tripCommandService;
        _logger = logger;
    }

    /// <summary>GET /trips — all, or filtered by userId.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TripResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTrips([FromQuery] string? userId)
    {
        var query = _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(userId)) query = query.Where(t => t.UserId == userId.Trim());
        var list = await query.ToListAsync();
        return Ok(list.Select(TripAssembler.ToTripFromEntity));
    }

    /// <summary>GET /trips/{id} — single trip.</summary>
    /// <param name="id">Trip id.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TripResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTripById(int id)
    {
        var entity = await _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().FindAsync(id);
        if (entity is null) return NotFound();
        return Ok(TripAssembler.ToTripFromEntity(entity));
    }

    /// <summary>POST /trips — create.</summary>
    /// <param name="resource">The create payload.</param>
    [HttpPost]
    [ProducesResponseType(typeof(TripResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateTrip([FromBody] CreateTripResource resource)
    {
        try
        {
            var command = TripAssembler.ToCreateTripCommand(resource);
            var duplicate = await _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().AnyAsync(t => t.TripId == command.TripId);
            if (duplicate) return Conflict("Trip id already exists.");
            var entity = new backend_bicigo.Trip.Domain.Model.Aggregates.Trip(command);
            await _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTripById), new { id = entity.Id }, TripAssembler.ToTripFromEntity(entity));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid trip payload");
            return BadRequest("Invalid trip fields.");
        }
    }

    /// <summary>
    ///     PUT /trips/{id} — update (used by startTrip/finishTrip store actions).
    ///     When the payload status is FINISHED, the trip is finalized server-side
    ///     (finishedAt + totalCost computed by the backend).
    /// </summary>
    /// <param name="id">Trip id.</param>
    /// <param name="resource">The update payload.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TripResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateTrip(int id, [FromBody] TripResource resource)
    {
        var entity = await _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().FindAsync(id);
        if (entity is null) return NotFound();

        string status;
        try
        {
            status = !string.IsNullOrWhiteSpace(resource.Status)
                ? TripAssembler.ToTripStatus(resource.Status)
                : entity.Status;
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid trip status");
            return BadRequest("Invalid trip status.");
        }

        if (status == backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.Finished)
        {
            // Idempotent: finishing an already finished trip returns the stored result.
            if (entity.Status == backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.Finished)
                return Ok(TripAssembler.ToTripFromEntity(entity));

            var finishResult = await _tripCommandService.Handle(
                new FinishTripCommand(id, resource.DistanceKm), HttpContext.RequestAborted);
            return finishResult.Fold<IActionResult>(
                trip => Ok(TripAssembler.ToTripFromEntity(trip)),
                ToFinishErrorActionResult);
        }

        if (status == backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.InProgress
            && entity.Status != backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.Finished)
        {
            try
            {
                entity.StartTrip(resource.StartedAt);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                _logger.LogWarning(ex, "Invalid trip update");
                return BadRequest("Invalid trip update.");
            }
        }

        return Ok(TripAssembler.ToTripFromEntity(entity));
    }

    /// <summary>
    ///     POST /trips/{id}/finish — finalizes an IN_PROGRESS trip.
    ///     The server stamps finishedAt and computes totalCost from the user plan/subscription.
    /// </summary>
    /// <param name="id">Trip id.</param>
    /// <param name="resource">The finish payload (distanceKm).</param>
    [HttpPost("{id:int}/finish")]
    [ProducesResponseType(typeof(TripResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> FinishTrip(int id, [FromBody] FinishTripResource resource)
    {
        if (resource?.DistanceKm is null) return BadRequest("distanceKm is required.");
        if (resource.DistanceKm < 0) return BadRequest("distanceKm cannot be negative.");

        var result = await _tripCommandService.Handle(
            TripAssembler.ToFinishTripCommand(id, resource), HttpContext.RequestAborted);

        return result.Fold<IActionResult>(
            trip => Ok(TripAssembler.ToTripFromEntity(trip)),
            ToFinishErrorActionResult);
    }

    /// <summary>Maps a <see cref="FinishTripError"/> to the matching HTTP response.</summary>
    private IActionResult ToFinishErrorActionResult(FinishTripError error) => error switch
    {
        FinishTripError.NotFound => NotFound(),
        FinishTripError.InvalidTransition => Conflict("Only an in-progress trip can be finished."),
        FinishTripError.InvalidDistance => BadRequest("distanceKm must be greater than or equal to zero."),
        _ => StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error while finishing the trip.")
    };

    /// <summary>DELETE /trips/{id}.</summary>
    /// <param name="id">Trip id.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTrip(int id)
    {
        var entity = await _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().FindAsync(id);
        if (entity is null) return NotFound();
        _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
