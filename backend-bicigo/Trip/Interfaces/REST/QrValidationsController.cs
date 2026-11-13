using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Trip.Domain.Model.Aggregates;
using backend_bicigo.Trip.Interfaces.REST.Resources;
using backend_bicigo.Trip.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Trip.Interfaces.REST;

/// <summary>
///     QR validations controller: GET /qr-validations, POST /qr-validations, GET /qr-validations/{id}.
///     Supports GET /qr-validations?tripId=.
/// </summary>
[ApiController]
[Route("api/v1/qr-validations")]
[Produces("application/json")]
public class QrValidationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<QrValidationsController> _logger;

    /// <summary>Initializes a new instance.</summary>
    public QrValidationsController(AppDbContext context, ILogger<QrValidationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>GET /qr-validations — all, or filtered by tripId.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<QrValidationResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQrValidations([FromQuery] string? tripId)
    {
        var query = _context.Set<QrValidation>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(tripId)) query = query.Where(q => q.TripId == tripId.Trim());
        var list = await query.ToListAsync();
        return Ok(list.Select(TripAssembler.ToQrFromEntity));
    }

    /// <summary>GET /qr-validations/{id} — single validation.</summary>
    /// <param name="id">Validation id.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(QrValidationResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetQrValidationById(int id)
    {
        var entity = await _context.Set<QrValidation>().FindAsync(id);
        if (entity is null) return NotFound();
        return Ok(TripAssembler.ToQrFromEntity(entity));
    }

    /// <summary>POST /qr-validations — create and validate against the persisted bicycle QR code.</summary>
    /// <param name="resource">The create payload.</param>
    [HttpPost]
    [ProducesResponseType(typeof(QrValidationResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateQrValidation([FromBody] CreateQrValidationResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = TripAssembler.ToCreateQrCommand(resource);
            var validations = _context.Set<QrValidation>();
            if (await validations.AnyAsync(q => q.ValidationId == command.ValidationId, cancellationToken))
                return Conflict("Validation id already exists.");

            var trip = await _context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>()
                .FirstOrDefaultAsync(t => t.TripId == command.TripId, cancellationToken);
            if (trip is null) return NotFound("Trip not found.");
            if (!string.Equals(trip.BicycleId, command.BicycleId, StringComparison.Ordinal))
                return BadRequest("The bicycle does not belong to the specified trip.");

            var bicycleQuery = _context.Set<Bicycle>();
            var bicycle = await bicycleQuery.FirstOrDefaultAsync(
                b => b.BikeCode == command.BicycleId || b.SerialNumber == command.BicycleId,
                cancellationToken);
            if (bicycle is null && int.TryParse(command.BicycleId, out var bicycleNumericId))
                bicycle = await bicycleQuery.FirstOrDefaultAsync(b => b.Id == bicycleNumericId, cancellationToken);
            if (bicycle is null) return NotFound("Bicycle not found.");

            var entity = new QrValidation(command);
            // The expected value always comes from the persisted bicycle, never from the client payload.
            entity.Validate(bicycle.QrCode);
            await validations.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            var response = TripAssembler.ToQrFromEntity(entity);
            return entity.Result == QrValidation.Results.Valid
                ? CreatedAtAction(nameof(GetQrValidationById), new { id = entity.Id }, response)
                : BadRequest(new { message = "The QR code is invalid for this bicycle.", validation = response });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid QR payload");
            return BadRequest("Invalid QR fields.");
        }
    }
}
