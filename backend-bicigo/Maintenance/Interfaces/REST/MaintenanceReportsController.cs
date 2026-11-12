using backend_bicigo.Maintenance.Domain.Model.Aggregates;
using backend_bicigo.Maintenance.Domain.Model.Commands;
using backend_bicigo.Maintenance.Interfaces.REST.Resources;
using backend_bicigo.Maintenance.Interfaces.REST.Transform;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Maintenance.Interfaces.REST;

[ApiController]
[Route("api/v1/maintenance-reports")]
[Produces("application/json")]
public class MaintenanceReportsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<MaintenanceReportsController> _logger;

    /// <summary>Initializes a new instance.</summary>
    public MaintenanceReportsController(AppDbContext context, ILogger<MaintenanceReportsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>GET /maintenance-reports — all, optionally filtered by bikeId/status.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MaintenanceReportResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReports([FromQuery] string? bikeId, [FromQuery] string? status)
    {
        var query = _context.Set<MaintenanceReport>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(bikeId)) query = query.Where(m => m.BikeId == bikeId.Trim());
        if (!string.IsNullOrWhiteSpace(status)) query = query.Where(m => m.Status == status.Trim().ToUpperInvariant());
        var list = await query.ToListAsync();
        return Ok(list.Select(MaintenanceAssembler.ToResourceFromEntity));
    }

    /// <summary>GET /maintenance-reports/{id} — single report.</summary>
    /// <param name="id">Report id.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MaintenanceReportResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReportById(int id)
    {
        var entity = await _context.Set<MaintenanceReport>().FindAsync(id);
        if (entity is null) return NotFound();
        return Ok(MaintenanceAssembler.ToResourceFromEntity(entity));
    }

    /// <summary>POST /maintenance-reports — create.</summary>
    /// <param name="resource">The create payload.</param>
    [HttpPost]
    [ProducesResponseType(typeof(MaintenanceReportResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateReport([FromBody] CreateMaintenanceReportResource resource)
    {
        var errors = MaintenanceReport.Validate(resource.BikeId, resource.ProblemType, resource.Description);
        if (errors.Count > 0) return BadRequest(errors);
        try
        {
            var command = MaintenanceAssembler.ToCommandFromResource(resource);
            var entity = new MaintenanceReport(command);
            var previous = await _context.Set<MaintenanceReport>()
                .Where(m => m.BikeId == entity.BikeId)
                .OrderByDescending(m => m.Id)
                .FirstOrDefaultAsync();
            if (previous is not null) entity.UpdateBikeStatus(previous.Status);
            await _context.Set<MaintenanceReport>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetReportById), new { id = entity.Id }, MaintenanceAssembler.ToResourceFromEntity(entity));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid maintenance payload");
            return BadRequest("Invalid maintenance report.");
        }
    }

    /// <summary>POST /maintenance-reports/{bikeId}/status — toggle bike status (updates all reports of that bike).</summary>
    /// <param name="bikeId">Bike reference.</param>
    /// <param name="resource">The status payload (expects { status }).</param>
    [HttpPost("{bikeId}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBikeStatus(string bikeId, [FromBody] UpdateBikeStatusResource resource)
    {
        if (string.IsNullOrWhiteSpace(resource.Status)) return BadRequest("Status is required.");
        try
        {
            var normalized = resource.Status.Trim().ToUpperInvariant();
            if (normalized is not (MaintenanceReport.BikeStatuses.Available or MaintenanceReport.BikeStatuses.Maintenance))
                return BadRequest("Status must be AVAILABLE or MAINTENANCE.");
            var list = await _context.Set<MaintenanceReport>()
                .Where(m => m.BikeId == bikeId.Trim())
                .ToListAsync();
            if (list.Count == 0) return NotFound();
            foreach (var report in list) report.UpdateBikeStatus(normalized);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid bike status transition");
            return BadRequest("Invalid status transition.");
        }
    }

    /// <summary>Update-status payload.</summary>
    public sealed class UpdateBikeStatusResource
    {
        /// <summary>New status AVAILABLE|MAINTENANCE.</summary>
        public string Status { get; set; } = string.Empty;
    }
}
