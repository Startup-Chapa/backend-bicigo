using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Billing.Interfaces.REST.Resources;
using backend_bicigo.Billing.Interfaces.REST.Transform;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Billing.Interfaces.REST;

/// <summary>
///     Payment methods controller: GET /payment-methods, POST /payment-methods.
///     Supports GET /payment-methods?userId=.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class PaymentMethodsController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>Initializes a new instance.</summary>
    public PaymentMethodsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>GET /payment-methods — all, or filtered by userId.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PaymentMethodResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaymentMethods([FromQuery] int? userId)
    {
        var query = _context.Set<PaymentMethod>().AsQueryable();
        if (userId.HasValue) query = query.Where(p => p.UserId == userId.Value);
        var list = await query.ToListAsync();
        return Ok(list.Select(BillingResourceFromEntityAssembler.ToPaymentMethodFromEntity));
    }

    /// <summary>POST /payment-methods — create.</summary>
    /// <param name="resource">The create payload.</param>
    [HttpPost]
    [ProducesResponseType(typeof(PaymentMethodResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePaymentMethod([FromBody] CreatePaymentMethodResource resource)
    {
        if (string.IsNullOrWhiteSpace(resource.Token)) return BadRequest("Token is required.");
        try
        {
            var command = BillingCommandsFromResourcesAssembler.ToPaymentMethodCommand(resource);
            var entity = new PaymentMethod(command);
            await _context.Set<PaymentMethod>().AddAsync(entity);
            await _context.SaveChangesAsync();
            var created = BillingResourceFromEntityAssembler.ToPaymentMethodFromEntity(entity);
            return CreatedAtAction(nameof(GetPaymentMethodById), new { id = created.Id }, created);
        }
        catch (ArgumentException)
        {
            return BadRequest("Invalid payment method.");
        }
    }

    /// <summary>GET /payment-methods/{id} — single method.</summary>
    /// <param name="id">Payment method id.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PaymentMethodResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPaymentMethodById(int id)
    {
        var entity = await _context.Set<PaymentMethod>().FindAsync(id);
        if (entity is null) return NotFound();
        return Ok(BillingResourceFromEntityAssembler.ToPaymentMethodFromEntity(entity));
    }
}
