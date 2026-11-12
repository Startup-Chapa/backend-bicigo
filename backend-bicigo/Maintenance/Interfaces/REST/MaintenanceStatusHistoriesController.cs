using System.ComponentModel.DataAnnotations;
using backend_bicigo.Maintenance.Domain.Model.Aggregates;
using backend_bicigo.Maintenance.Interfaces.REST.Resources;
using backend_bicigo.Maintenance.Interfaces.REST.Transform;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Maintenance.Interfaces.REST;

[ApiController]
[Route("api/v1/maintenance-status-histories")]
[Produces("application/json")]
public class MaintenanceStatusHistoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>Initializes a new instance.</summary>
    public MaintenanceStatusHistoriesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>GET /maintenance-status-histories — all, or filtered.</summary>
    [HttpGet]
    public async Task<IActionResult> GetHistories([FromQuery] string? bikeId, [FromQuery] int? maintenanceReportId)
    {
        var query = _context.Set<MaintenanceStatusHistory>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(bikeId)) query = query.Where(h => h.BikeId == bikeId.Trim());
        if (maintenanceReportId.HasValue) query = query.Where(h => h.MaintenanceReportId == maintenanceReportId.Value);
        var list = await query.ToListAsync();
        return Ok(list.Select(MaintenanceStatusHistoryAssembler.ToResourceFromEntity));
    }

    /// <summary>GET /maintenance-status-histories/{id}.</summary>
    /// <param name="id">History id.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetHistoryById(int id)
    {
        var entity = await _context.Set<MaintenanceStatusHistory>().FindAsync(id);
        if (entity is null) return NotFound();
        return Ok(MaintenanceStatusHistoryAssembler.ToResourceFromEntity(entity));
    }

    /// <summary>POST /maintenance-status-histories — create.</summary>
    /// <param name="resource">The create payload.</param>
    [HttpPost]
    public async Task<IActionResult> CreateHistory([FromBody] CreateMaintenanceStatusHistoryResource resource)
    {
        try
        {
            var entity = new MaintenanceStatusHistory(
                resource.MaintenanceReportId, resource.BikeId, resource.PreviousStatus, resource.NewStatus, resource.ChangedAt);
            await _context.Set<MaintenanceStatusHistory>().AddAsync(entity);
            await _context.SaveChangesAsync();
            var created = MaintenanceStatusHistoryAssembler.ToResourceFromEntity(entity);
            return CreatedAtAction(nameof(GetHistoryById), new { id = created.Id }, created);
        }
        catch (ArgumentException)
        {
            return BadRequest("Invalid history fields.");
        }
    }

    /// <summary>Create payload.</summary>
    public sealed class CreateMaintenanceStatusHistoryResource
    {
        /// <summary>Report id.</summary>
        [Range(1, int.MaxValue)] public int MaintenanceReportId { get; set; }

        /// <summary>Bike reference.</summary>
        [Required] public string BikeId { get; set; } = string.Empty;

        /// <summary>Previous status.</summary>
        [Required] public string PreviousStatus { get; set; } = string.Empty;

        /// <summary>New status.</summary>
        [Required] public string NewStatus { get; set; } = string.Empty;

        /// <summary>Changed timestamp ISO (optional).</summary>
        public string? ChangedAt { get; set; }
    }
}
