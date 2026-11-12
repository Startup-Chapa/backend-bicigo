using System.ComponentModel.DataAnnotations;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Trip.Domain.Model.Aggregates;
using backend_bicigo.Trip.Interfaces.REST.Resources;
using backend_bicigo.Trip.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Trip.Interfaces.REST;

/// <summary>
///     Trip photo evidences controller: GET /trip-photo-evidences (?tripId=), POST /trip-photo-evidences.
/// </summary>
[ApiController]
[Route("api/v1/trip-photo-evidences")]
[Produces("application/json")]
public class TripPhotoEvidencesController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>Initializes a new instance.</summary>
    public TripPhotoEvidencesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>GET /trip-photo-evidences — all, or filtered by tripId.</summary>
    [HttpGet]
    public async Task<IActionResult> GetPhotoEvidences([FromQuery] string? tripId)
    {
        var query = _context.Set<TripPhotoEvidence>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(tripId)) query = query.Where(p => p.TripId == tripId.Trim());
        var list = await query.ToListAsync();
        return Ok(list.Select(TripExtrasAssembler.ToPhotoFromEntity));
    }

    /// <summary>GET /trip-photo-evidences/{id}.</summary>
    /// <param name="id">Photo evidence id.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPhotoEvidenceById(int id)
    {
        var entity = await _context.Set<TripPhotoEvidence>().FindAsync(id);
        if (entity is null) return NotFound();
        return Ok(TripExtrasAssembler.ToPhotoFromEntity(entity));
    }

    /// <summary>POST /trip-photo-evidences — create.</summary>
    /// <param name="resource">The create payload.</param>
    [HttpPost]
    public async Task<IActionResult> CreatePhotoEvidence([FromBody] CreateTripPhotoEvidenceResource resource)
    {
        try
        {
            var tripExists = await _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>()
                .AnyAsync(t => t.TripId == resource.TripId.Trim());
            if (!tripExists) return NotFound("Trip not found.");

            var entity = new TripPhotoEvidence(resource.TripId, resource.PhotoUrl, resource.CapturedAt);
            await _context.Set<TripPhotoEvidence>().AddAsync(entity);
            await _context.SaveChangesAsync();
            var created = TripExtrasAssembler.ToPhotoFromEntity(entity);
            return CreatedAtAction(nameof(GetPhotoEvidenceById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>Create payload.</summary>
    public sealed class CreateTripPhotoEvidenceResource
    {
        /// <summary>Trip reference.</summary>
        [Required] public string TripId { get; set; } = string.Empty;

        /// <summary>Photo URL.</summary>
        [Required] public string PhotoUrl { get; set; } = string.Empty;

        /// <summary>Captured timestamp ISO (optional).</summary>
        public string? CapturedAt { get; set; }
    }
}
