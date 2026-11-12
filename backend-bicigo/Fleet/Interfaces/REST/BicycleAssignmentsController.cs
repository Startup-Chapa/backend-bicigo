using System.ComponentModel.DataAnnotations;
using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Fleet.Interfaces.REST.Resources;
using backend_bicigo.Fleet.Interfaces.REST.Transform;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Fleet.Interfaces.REST;

/// <summary>
///     Bicycle assignments controller: GET /bicycle-assignments (?bicycleId=&amp;bikePointId=), POST /bicycle-assignments.
/// </summary>
[ApiController]
[Route("api/v1/bicycle-assignments")]
[Produces("application/json")]
public class BicycleAssignmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>Initializes a new instance.</summary>
    public BicycleAssignmentsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>GET /bicycle-assignments — all, or filtered.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAssignments([FromQuery] int? bicycleId, [FromQuery] int? bikePointId)
    {
        var query = _context.Set<BicycleAssignment>().AsQueryable();
        if (bicycleId.HasValue) query = query.Where(a => a.BicycleId == bicycleId.Value);
        if (bikePointId.HasValue) query = query.Where(a => a.BikePointId == bikePointId.Value);
        var list = await query.ToListAsync();
        return Ok(list.Select(BicycleAssignmentAssembler.ToResourceFromEntity));
    }

    /// <summary>GET /bicycle-assignments/{id}.</summary>
    /// <param name="id">Assignment id.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAssignmentById(int id)
    {
        var entity = await _context.Set<BicycleAssignment>().FindAsync(id);
        if (entity is null) return NotFound();
        return Ok(BicycleAssignmentAssembler.ToResourceFromEntity(entity));
    }

    /// <summary>POST /bicycle-assignments — create.</summary>
    /// <param name="resource">The create payload.</param>
    [HttpPost]
    public async Task<IActionResult> CreateAssignment([FromBody] CreateBicycleAssignmentResource resource)
    {
        try
        {
            var entity = new BicycleAssignment(resource.BicycleId, resource.BikePointId, resource.Action, resource.AssignedAt);
            await _context.Set<BicycleAssignment>().AddAsync(entity);
            await _context.SaveChangesAsync();
            var created = BicycleAssignmentAssembler.ToResourceFromEntity(entity);
            return CreatedAtAction(nameof(GetAssignmentById), new { id = created.Id }, created);
        }
        catch (ArgumentException)
        {
            return BadRequest("Invalid assignment fields.");
        }
    }

    /// <summary>Create payload.</summary>
    public sealed class CreateBicycleAssignmentResource
    {
        /// <summary>Bicycle id.</summary>
        [Range(1, int.MaxValue)] public int BicycleId { get; set; }

        /// <summary>Bike-point id.</summary>
        [Range(1, int.MaxValue)] public int BikePointId { get; set; }

        /// <summary>Action ASSIGN|UNLOCK|REMOVE.</summary>
        [Required] public string Action { get; set; } = string.Empty;

        /// <summary>Assigned timestamp ISO (optional).</summary>
        public string? AssignedAt { get; set; }
    }
}
