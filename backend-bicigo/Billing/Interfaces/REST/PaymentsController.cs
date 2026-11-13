using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Billing.Domain.Model.Queries;
using backend_bicigo.Billing.Interfaces.REST.Resources;
using backend_bicigo.Billing.Interfaces.REST.Transform;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Billing.Interfaces.REST;

/// <summary>
///     Payments controller: GET /payments, POST /payments.
///     Supports GET /payments?userId= and ?subscriptionId= for chargeTrip/topUpBalance flows.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class PaymentsController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>Initializes a new instance.</summary>
    public PaymentsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>GET /payments — all, or filtered by userId/subscriptionId.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PaymentResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPayments([FromQuery] int? userId, [FromQuery] int? subscriptionId)
    {
        var query = _context.Set<Payment>().AsQueryable();
        if (userId.HasValue) query = query.Where(p => p.UserId == userId.Value);
        if (subscriptionId.HasValue) query = query.Where(p => p.SubscriptionId == subscriptionId.Value);
        var list = await query.ToListAsync();
        return Ok(list.Select(BillingResourceFromEntityAssembler.ToPaymentFromEntity));
    }

    /// <summary>POST /payments — create (CHARGE or TOPUP).</summary>
    /// <param name="resource">The create payload.</param>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentResource resource)
    {
        try
        {
            var command = BillingCommandsFromResourcesAssembler.ToPaymentCommand(resource);
            var entity = new Payment(command);
            await _context.Set<Payment>().AddAsync(entity);
            await _context.SaveChangesAsync();
            var created = BillingResourceFromEntityAssembler.ToPaymentFromEntity(entity);
            return CreatedAtAction(nameof(GetPaymentById), new { id = created.Id }, created);
        }
        catch (ArgumentException)
        {
            return BadRequest("Invalid payment type.");
        }
    }

    /// <summary>GET /payments/{id} — single payment.</summary>
    /// <param name="id">Payment id.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PaymentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        var entity = await _context.Set<Payment>().FindAsync(id);
        if (entity is null) return NotFound();
        return Ok(BillingResourceFromEntityAssembler.ToPaymentFromEntity(entity));
    }
}
