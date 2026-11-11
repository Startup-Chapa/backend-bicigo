using System.ComponentModel.DataAnnotations;
using backend_bicigo.Iam.Domain.Model.Aggregates;
using backend_bicigo.Iam.Interfaces.REST.Resources;
using backend_bicigo.Iam.Interfaces.REST.Transform;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Iam.Interfaces.REST;

/// <summary>
///     User notifications controller: GET /user-notifications (?userId=), POST /user-notifications,
///     PATCH /user-notifications/{id}/read.
/// </summary>
[ApiController]
[Route("api/v1/user-notifications")]
[Produces("application/json")]
public class UserNotificationsController : ControllerBase
{
    private readonly AppDbContext _context;

    /// <summary>Initializes a new instance.</summary>
    public UserNotificationsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>GET /user-notifications — all, or filtered by userId.</summary>
    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] int? userId)
    {
        var query = _context.Set<UserNotification>().AsQueryable();
        if (userId.HasValue) query = query.Where(n => n.UserId == userId.Value);
        var list = await query.ToListAsync();
        return Ok(list.Select(UserNotificationAssembler.ToResourceFromEntity));
    }

    /// <summary>GET /user-notifications/{id}.</summary>
    /// <param name="id">Notification id.</param>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetNotificationById(int id)
    {
        var entity = await _context.Set<UserNotification>().FindAsync(id);
        if (entity is null) return NotFound();
        return Ok(UserNotificationAssembler.ToResourceFromEntity(entity));
    }

    /// <summary>POST /user-notifications — create.</summary>
    /// <param name="resource">The create payload.</param>
    [HttpPost]
    public async Task<IActionResult> CreateNotification([FromBody] CreateUserNotificationResource resource)
    {
        try
        {
            var entity = new UserNotification(resource.UserId, resource.Title, resource.Message, resource.Type);
            await _context.Set<UserNotification>().AddAsync(entity);
            await _context.SaveChangesAsync();
            var created = UserNotificationAssembler.ToResourceFromEntity(entity);
            return CreatedAtAction(nameof(GetNotificationById), new { id = created.Id }, created);
        }
        catch (ArgumentException)
        {
            return BadRequest("Invalid notification fields.");
        }
    }

    /// <summary>PATCH /user-notifications/{id}/read — mark as read.</summary>
    /// <param name="id">Notification id.</param>
    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var entity = await _context.Set<UserNotification>().FindAsync(id);
        if (entity is null) return NotFound();
        entity.MarkAsRead();
        await _context.SaveChangesAsync();
        return Ok(UserNotificationAssembler.ToResourceFromEntity(entity));
    }

    /// <summary>Create payload.</summary>
    public sealed class CreateUserNotificationResource
    {
        /// <summary>User id.</summary>
        [Range(1, int.MaxValue)] public int UserId { get; set; }

        /// <summary>Title.</summary>
        [Required] public string Title { get; set; } = string.Empty;

        /// <summary>Message.</summary>
        [Required] public string Message { get; set; } = string.Empty;

        /// <summary>Type INFO|ALERT|SUCCESS.</summary>
        public string? Type { get; set; }
    }
}