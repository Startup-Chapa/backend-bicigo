using System.ComponentModel.DataAnnotations;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Trip.Domain.Model.Aggregates;
using backend_bicigo.Trip.Interfaces.REST.Resources;
using backend_bicigo.Trip.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Trip.Interfaces.REST;

/// <summary>
///     Trip route points controller: GET /trip-route-points (?tripId=), POST /trip-route-points.
/// </summary>
[ApiController]
[Route("api/v1/trip-route-points")]
[Produces("application/json")]
public class TripRoutePointsController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>Initializes a new instance.</summary>
    public TripRoutePointsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>GET /trip-route-points — all, or filtered by tripId (ordered by sequence).</summary>
    [HttpGet]
    public async Task<IActionResult> GetRoutePoints([FromQuery] string? tripId)
    {
        var query = _context.Set<TripRoutePoint>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(tripId)) query = query.Where(p => p.TripId == tripId.Trim());
        var list = await query.OrderBy(p => p.Sequence).ToListAsync();
        return Ok(list.Select(TripExtrasAssembler.ToRoutePointFromEntity));
    }

    /// <summary>GET /trip-route-points/{id}.</summary>
    /// <param name="id">Route point id.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRoutePointById(int id)
    {
        var entity = await _context.Set<TripRoutePoint>().FindAsync(id);
        if (entity is null) return NotFound();
        return Ok(TripExtrasAssembler.ToRoutePointFromEntity(entity));
    }

    /// <summary>POST /trip-route-points — create.</summary>
    /// <param name="resource">The create payload.</param>
    [HttpPost]
    public async Task<IActionResult> CreateRoutePoint([FromBody] CreateTripRoutePointResource resource)
    {
        try
        {
            var trip = await _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>()
                .FirstOrDefaultAsync(t => t.TripId == resource.TripId.Trim());
            if (trip is null) return NotFound("Trip not found.");
            if (trip.Status != backend_bicigo.Trip.Domain.Model.Aggregates.Trip.Statuses.InProgress)
                return Conflict("Route points can only be recorded for an in-progress trip.");

            var duplicateSequence = await _context.Set<TripRoutePoint>()
                .AnyAsync(p => p.TripId == resource.TripId.Trim() && p.Sequence == resource.Sequence);
            if (duplicateSequence) return Conflict("A route point with this sequence already exists for the trip.");

            var entity = new TripRoutePoint(resource.TripId, resource.Latitude, resource.Longitude, resource.Sequence, resource.RecordedAt);
            await _context.Set<TripRoutePoint>().AddAsync(entity);
            await _context.SaveChangesAsync();
            var created = TripExtrasAssembler.ToRoutePointFromEntity(entity);
            return CreatedAtAction(nameof(GetRoutePointById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>Create payload.</summary>
    public sealed class CreateTripRoutePointResource
    {
        /// <summary>Trip reference.</summary>
        [Required] public string TripId { get; set; } = string.Empty;

        /// <summary>Latitude.</summary>
        public double Latitude { get; set; }

        /// <summary>Longitude.</summary>
        public double Longitude { get; set; }

        /// <summary>Order in the path.</summary>
        [Range(0, int.MaxValue)] public int Sequence { get; set; }

        /// <summary>Recorded timestamp ISO (optional).</summary>
        public string? RecordedAt { get; set; }
    }
}
