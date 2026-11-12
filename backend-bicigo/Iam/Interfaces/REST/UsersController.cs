using System.Net.Mime;
using backend_bicigo.Iam.Application.Services;
using backend_bicigo.Iam.Domain.Model.Commands;
using backend_bicigo.Iam.Domain.Model.Queries;
using backend_bicigo.Iam.Domain.Repositories;
using backend_bicigo.Iam.Interfaces.REST.Resources;
using backend_bicigo.Iam.Interfaces.REST.Transform;
using backend_bicigo.Shared.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend_bicigo.Iam.Interfaces.REST;

/// <summary>
///     Users controller: POST /users, GET /users, GET /users/{id}, PATCH /users/{id}, DELETE /users/{id}.
/// </summary>
/// <remarks>
///     GET /users?email= returns an array (empty when none) to mirror json-server and the frontend
///     IamApi.login filter behaviour.
/// </remarks>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class UsersController : ControllerBase
{
    private readonly IUserCommandService _commandService;
    private readonly IUserQueryService _queryService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UsersController> _logger;

    /// <summary>Initializes a new instance of <see cref="UsersController"/>.</summary>
    public UsersController(
        IUserCommandService commandService,
        IUserQueryService queryService,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<UsersController> logger)
    {
        _commandService = commandService;
        _queryService = queryService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>POST /users — register.</summary>
    /// <param name="resource">The create payload.</param>
    [HttpPost]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserResource resource)
    {
        RegisterUserCommand command;
        try
        {
            command = CreateUserCommandFromResourceAssembler.ToCommandFromResource(resource);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid create-user payload");
            return BadRequest("Invalid e-mail or plan.");
        }

        var result = await _commandService.Handle(command);
        return ActionResultFromCreateUserResultAssembler.ToActionResultFromCreateUserResult(
            result, this, nameof(GetUserById));
    }

    /// <summary>GET /users — all users; GET /users?email= — filtered array.</summary>
    /// <param name="email">Optional e-mail filter.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers([FromQuery] string? email)
    {
        IEnumerable<UserResource> resources;
        if (string.IsNullOrWhiteSpace(email))
        {
            var all = await _userRepository.ListAsync();
            resources = all.Select(UserResourceFromEntityAssembler.ToResourceFromEntity);
        }
        else
        {
            var filtered = await _queryService.Handle(new GetUsersByEmailQuery(email));
            resources = filtered.Select(UserResourceFromEntityAssembler.ToResourceFromEntity);
        }

        return Ok(resources);
    }

    /// <summary>GET /users/{id} — single user.</summary>
    /// <param name="id">User id.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _queryService.Handle(new GetUserByIdQuery(id));
        if (user is null) return NotFound();
        return Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(user));
    }

    /// <summary>PATCH /users/{id} — partial update (updateProfile / balance / plan sync).</summary>
    /// <param name="id">User id.</param>
    /// <param name="resource">The patch payload.</param>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(UserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserResource resource)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user is null) return NotFound();
        user.ApplyUpdate(new UpdateUserCommand(
            Email: resource.Email,
            Password: resource.Password,
            Username: resource.Username,
            PhoneNumber: resource.PhoneNumber,
            Plan: resource.Plan,
            Balance: resource.Balance,
            Role: resource.Role,
            Location: resource.Location));
        _userRepository.Update(user);
        await _unitOfWork.CompleteAsync();
        return Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(user));
    }

    /// <summary>DELETE /users/{id}.</summary>
    /// <param name="id">User id.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _userRepository.FindByIdAsync(id);
        if (user is null) return NotFound();
        _userRepository.Remove(user);
        await _unitOfWork.CompleteAsync();
        return NoContent();
    }
}