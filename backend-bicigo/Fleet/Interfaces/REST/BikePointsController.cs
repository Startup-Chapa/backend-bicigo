using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Fleet.Domain.Model.Commands;
using backend_bicigo.Fleet.Interfaces.REST.Resources;
using backend_bicigo.Fleet.Interfaces.REST.Transform;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Fleet.Interfaces.REST;

/// <summary>
///     Bike points controller: GET /bike-points, GET /bike-points/{id}, POST /bike-points, PUT /bike-points/{id}, DELETE /bike-points/{id}.
///     Route is explicit kebab-case to match the frontend endpoint.
/// </summary>
[ApiController]
[Route("api/v1/bike-points")]
[Produces("application/json")]
public class BikePointsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<BikePointsController> _logger;

    /// <summary>Initializes a new instance.</summary>
    public BikePointsController(AppDbContext context, ILogger<BikePointsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>GET /bike-points — all stations.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BikePointResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBikePoints()
    {
        var list = await _context.Set<BikePoint>().ToListAsync();
        return Ok(list.Select(FleetAssembler.ToBikePointFromEntity));
    }

    /// <summary>GET /bike-points/{id} — single station.</summary>
    /// <param name="id">Bike-point id.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BikePointResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBikePointById(int id)
    {
        var entity = await _context.Set<BikePoint>().FindAsync(id);
        if (entity is null) return NotFound();
        return Ok(FleetAssembler.ToBikePointFromEntity(entity));
    }

    /// <summary>POST /bike-points — create.</summary>
    /// <param name="resource">The create payload.</param>
    [HttpPost]
    [ProducesResponseType(typeof(BikePointResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBikePoint([FromBody] CreateBikePointResource resource)
    {
        try
        {
            var command = FleetAssembler.ToRegisterBikePointCommand(resource);
            var entity = new BikePoint(command);
            await _context.Set<BikePoint>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBikePointById), new { id = entity.Id }, FleetAssembler.ToBikePointFromEntity(entity));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid bike-point payload");
            return BadRequest("Invalid bike-point fields.");
        }
    }

    /// <summary>PUT /bike-points/{id} — update.</summary>
    /// <param name="id">Bike-point id.</param>
    /// <param name="resource">The payload.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(BikePointResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBikePoint(int id, [FromBody] CreateBikePointResource resource)
    {
        var entity = await _context.Set<BikePoint>().FindAsync(id);
        if (entity is null) return NotFound();
        try
        {
            entity.ApplyUpdate(new UpdateBikePointCommand(
                Name: resource.Name,
                District: resource.District,
                Address: resource.Address,
                Latitude: resource.Latitude,
                Longitude: resource.Longitude,
                Capacity: resource.Capacity));
            await _context.SaveChangesAsync();
            return Ok(FleetAssembler.ToBikePointFromEntity(entity));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid bike-point update");
            return BadRequest("Invalid bike-point fields.");
        }
    }

    /// <summary>DELETE /bike-points/{id} — blocked when bicycles are docked.</summary>
    /// <param name="id">Bike-point id.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteBikePoint(int id)
    {
        var entity = await _context.Set<BikePoint>().FindAsync(id);
        if (entity is null) return NotFound();
        if (entity.CurrentBicyclesCount > 0) return Conflict("Bike point still has docked bicycles.");
        _context.Set<BikePoint>().Remove(entity);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
