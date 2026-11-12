using backend_bicigo.Fleet.Domain.Model.Commands;
using backend_bicigo.Fleet.Interfaces.REST.Resources;
using backend_bicigo.Fleet.Interfaces.REST.Transform;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Fleet.Domain.Model.Aggregates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Fleet.Interfaces.REST;

/// <summary>
///     Bicycles controller: GET /bicycles, GET /bicycles/{id}, POST /bicycles, PUT /bicycles/{id}, DELETE /bicycles/{id}.
///     Route is kebab-case to match the frontend (bike-points pattern).
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class BicyclesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<BicyclesController> _logger;

    /// <summary>Initializes a new instance.</summary>
    public BicyclesController(AppDbContext context, ILogger<BicyclesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>GET /bicycles — all bicycles.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BicycleResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBicycles()
    {
        var list = await _context.Set<Bicycle>().ToListAsync();
        return Ok(list.Select(FleetAssembler.ToBicycleFromEntity));
    }

    /// <summary>GET /bicycles/{id} — single bicycle.</summary>
    /// <param name="id">Bicycle id.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BicycleResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBicycleById(int id)
    {
        var entity = await _context.Set<Bicycle>().FindAsync(id);
        if (entity is null) return NotFound();
        return Ok(FleetAssembler.ToBicycleFromEntity(entity));
    }

    /// <summary>POST /bicycles — create (uses the fleet command service logic inline).</summary>
    /// <param name="resource">The create payload.</param>
    [HttpPost]
    [ProducesResponseType(typeof(BicycleResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateBicycle([FromBody] CreateBicycleResource resource)
    {
        try
        {
            var command = FleetAssembler.ToRegisterBicycleCommand(resource);
            var duplicate = await _context.Set<Bicycle>()
                .AnyAsync(b => b.BikeCode == command.BikeCode || b.SerialNumber == command.SerialNumber);
            if (duplicate) return Conflict("Bike code or serial already exists.");
            var entity = new Bicycle(command);
            await _context.Set<Bicycle>().AddAsync(entity);
            if (command.CurrentBikePointId.HasValue)
            {
                var point = await _context.Set<BikePoint>().FindAsync(command.CurrentBikePointId.Value);
                point?.AddBicycle();
            }
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBicycleById), new { id = entity.Id }, FleetAssembler.ToBicycleFromEntity(entity));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid bicycle payload");
            return BadRequest("Invalid bicycle fields.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Bike point cannot accept bicycle");
            return BadRequest("Bike point cannot accept this bicycle.");
        }
    }

    /// <summary>PUT /bicycles/{id} — full-ish update (frontend uses PUT via update(id,data)).</summary>
    /// <param name="id">Bicycle id.</param>
    /// <param name="resource">The payload (uses CreateBicycleResource shape + optional status).</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(BicycleResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBicycle(int id, [FromBody] CreateBicycleResource resource)
    {
        var entity = await _context.Set<Bicycle>().FindAsync(id);
        if (entity is null) return NotFound();
        try
        {
            entity.ApplyUpdate(new UpdateBicycleCommand(
                SerialNumber: resource.SerialNumber,
                BikeCode: resource.BikeCode,
                QrCode: resource.QrCode,
                Model: resource.Model,
                CurrentBikePointId: resource.CurrentBikePointId));
            await _context.SaveChangesAsync();
            return Ok(FleetAssembler.ToBicycleFromEntity(entity));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid bicycle update");
            return BadRequest("Invalid bicycle fields.");
        }
    }

    /// <summary>DELETE /bicycles/{id}.</summary>
    /// <param name="id">Bicycle id.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBicycle(int id)
    {
        var entity = await _context.Set<Bicycle>().FindAsync(id);
        if (entity is null) return NotFound();
        if (entity.CurrentBikePointId.HasValue)
        {
            var point = await _context.Set<BikePoint>().FindAsync(entity.CurrentBikePointId.Value);
            point?.RemoveBicycle();
        }
        _context.Set<Bicycle>().Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
