using System.ComponentModel.DataAnnotations;
using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Billing.Interfaces.REST.Resources;
using backend_bicigo.Billing.Interfaces.REST.Transform;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Billing.Interfaces.REST;

/// <summary>
///     Plan features controller: GET /plan-features (?planId=), POST /plan-features.
/// </summary>
[ApiController]
[Route("api/v1/plan-features")]
[Produces("application/json")]
public class PlanFeaturesController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>Initializes a new instance.</summary>
    public PlanFeaturesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>GET /plan-features — all, or filtered by planId.</summary>
    [HttpGet]
    public async Task<IActionResult> GetPlanFeatures([FromQuery] int? planId)
    {
        var query = _context.Set<PlanFeature>().AsQueryable();
        if (planId.HasValue) query = query.Where(f => f.PlanId == planId.Value);
        var list = await query.ToListAsync();
        return Ok(list.Select(PlanFeatureAssembler.ToResourceFromEntity));
    }

    /// <summary>GET /plan-features/{id}.</summary>
    /// <param name="id">Feature id.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPlanFeatureById(int id)
    {
        var entity = await _context.Set<PlanFeature>().FindAsync(id);
        if (entity is null) return NotFound();
        return Ok(PlanFeatureAssembler.ToResourceFromEntity(entity));
    }

    /// <summary>POST /plan-features — create.</summary>
    /// <param name="resource">The create payload.</param>
    [HttpPost]
    public async Task<IActionResult> CreatePlanFeature([FromBody] CreatePlanFeatureResource resource)
    {
        if (string.IsNullOrWhiteSpace(resource.Name)) return BadRequest("Name is required.");
        var entity = new PlanFeature(resource.PlanId, resource.Name);
        await _context.Set<PlanFeature>().AddAsync(entity);
        await _context.SaveChangesAsync();
        var created = PlanFeatureAssembler.ToResourceFromEntity(entity);
        return CreatedAtAction(nameof(GetPlanFeatureById), new { id = created.Id }, created);
    }

    /// <summary>Create payload.</summary>
    public sealed class CreatePlanFeatureResource
    {
        /// <summary>Plan id.</summary>
        [Range(1, int.MaxValue)] public int PlanId { get; set; }

        /// <summary>Feature name.</summary>
        [Required] public string Name { get; set; } = string.Empty;
    }
}
