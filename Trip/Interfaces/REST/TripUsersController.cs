using System.ComponentModel.DataAnnotations;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Trip.Domain.Model.Aggregates;
using backend_bicigo.Trip.Interfaces.REST.Resources;
using backend_bicigo.Trip.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Trip.Interfaces.REST;

/// <summary>
///     Trip users controller: GET /trip-users (?userId=), POST /trip-users.
/// </summary>
[ApiController]
[Route("api/v1/trip-users")]
[Produces("application/json")]
public class TripUsersController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>Initializes a new instance.</summary>
    public TripUsersController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>GET /trip-users — all, or filtered by userId.</summary>
    [HttpGet]
    public async Task<IActionResult> GetTripUsers([FromQuery] string? userId)
    {
        var query = _context.Set<TripUser>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(userId)) query = query.Where(t => t.UserId == userId.Trim());
        var list = await query.ToListAsync();
        return Ok(list.Select(TripExtrasAssembler.ToTripUserFromEntity));
    }

    /// <summary>GET /trip-users/{id}.</summary>
    /// <param name="id">Trip user id.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetTripUserById(int id)
    {
        var entity = await _context.Set<TripUser>().FindAsync(id);
        if (entity is null) return NotFound();
        return Ok(TripExtrasAssembler.ToTripUserFromEntity(entity));
    }

    /// <summary>POST /trip-users — create.</summary>
    /// <param name="resource">The create payload.</param>
    [HttpPost]
    public async Task<IActionResult> CreateTripUser([FromBody] CreateTripUserResource resource)
    {
        try
        {
            var normalizedUserId = resource.UserId.Trim();
            var normalizedEmail = resource.Email.Trim().ToLowerInvariant();
            var duplicate = await _context.Set<TripUser>().AnyAsync(
                t => t.UserId == normalizedUserId || t.Email == normalizedEmail);
            if (duplicate) return Conflict("A trip user with this user id or email already exists.");

            var entity = new TripUser(resource.UserId, resource.Name, resource.Email, resource.Role, resource.Active);
            await _context.Set<TripUser>().AddAsync(entity);
            await _context.SaveChangesAsync();
            var created = TripExtrasAssembler.ToTripUserFromEntity(entity);
            return CreatedAtAction(nameof(GetTripUserById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>Create payload.</summary>
    public sealed class CreateTripUserResource
    {
        /// <summary>User reference.</summary>
        [Required] public string UserId { get; set; } = string.Empty;

        /// <summary>Display name.</summary>
        [Required] public string Name { get; set; } = string.Empty;

        /// <summary>E-mail.</summary>
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;

        /// <summary>Role.</summary>
        public string? Role { get; set; }

        /// <summary>Whether active.</summary>
        public bool Active { get; set; } = true;
    }
}
