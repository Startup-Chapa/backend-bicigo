using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Billing.Domain.Model.Queries;
using backend_bicigo.Billing.Interfaces.REST.Resources;
using backend_bicigo.Billing.Interfaces.REST.Transform;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Billing.Interfaces.REST;

/// <summary>
///     Subscriptions controller: GET /subscriptions, POST /subscriptions, GET /subscriptions/{id}.
///     Supports GET /subscriptions?userId= for findPayPerUseSubscription.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class SubscriptionsController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>Initializes a new instance.</summary>
    public SubscriptionsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>GET /subscriptions — all, or filtered by userId.</summary>
    /// <param name="userId">Optional user filter.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SubscriptionResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubscriptions([FromQuery] int? userId)
    {
        var query = _context.Set<Subscription>().AsQueryable();
        if (userId.HasValue) query = query.Where(s => s.UserId == userId.Value);
        var list = await query.ToListAsync();
        return Ok(list.Select(BillingResourceFromEntityAssembler.ToSubscriptionFromEntity));
    }

    /// <summary>GET /subscriptions/{id} — single subscription.</summary>
    /// <param name="id">Subscription id.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SubscriptionResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSubscriptionById(int id)
    {
        var entity = await _context.Set<Subscription>().FindAsync(id);
        if (entity is null) return NotFound();
        return Ok(BillingResourceFromEntityAssembler.ToSubscriptionFromEntity(entity));
    }

    /// <summary>POST /subscriptions — create.</summary>
    /// <param name="resource">The create payload.</param>
    [HttpPost]
    [ProducesResponseType(typeof(SubscriptionResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionResource resource)
    {
        try
        {
            var command = BillingCommandsFromResourcesAssembler.ToSubscriptionCommand(resource);
            var entity = new Subscription(command);
            await _context.Set<Subscription>().AddAsync(entity);
            await _context.SaveChangesAsync();
            var created = BillingResourceFromEntityAssembler.ToSubscriptionFromEntity(entity);
            return CreatedAtAction(nameof(GetSubscriptionById), new { id = created.Id }, created);
        }
        catch (ArgumentException)
        {
            return BadRequest("Invalid plan type.");
        }
    }
}
