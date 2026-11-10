using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Billing.Domain.Model.Queries;
using backend_bicigo.Billing.Interfaces.REST.Resources;
using backend_bicigo.Billing.Interfaces.REST.Transform;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Billing.Interfaces.REST;

/// <summary>
///     Plans controller: GET /plans (fetchPlans), GET /plans/{id}.
/// </summary>
/// <remarks>
///     Routes must stay lower-case (plans) to match the frontend BaseEndpoint paths.
///     Controllers live under api/v1 in this API; see Program.cs route note.
/// </remarks>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class PlansController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>Initializes a new instance.</summary>
    public PlansController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>GET /plans — all plans.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PlanResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlans()
    {
        var plans = await _context.Set<Plan>().ToListAsync();
        return Ok(plans.Select(BillingResourceFromEntityAssembler.ToPlanFromEntity));
    }

    /// <summary>GET /plans/{id} — single plan.</summary>
    /// <param name="id">Plan id.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PlanResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlanById(int id)
    {
        var plan = await _context.Set<Plan>().FindAsync(id);
        if (plan is null) return NotFound();
        return Ok(BillingResourceFromEntityAssembler.ToPlanFromEntity(plan));
    }
}
