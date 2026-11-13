using backend_bicigo.Iam.Application.Errors;
using backend_bicigo.Iam.Domain.Model.Aggregates;
using backend_bicigo.Iam.Interfaces.REST.Resources;
using backend_bicigo.Iam.Interfaces.REST.Transform;
using backend_bicigo.Shared.Application.Patterns;
using Microsoft.AspNetCore.Mvc;

namespace backend_bicigo.Iam.Interfaces.REST.Transform;

/// <summary>
///     Assembles an HTTP action result from the create-user result.
/// </summary>
public static class ActionResultFromCreateUserResultAssembler
{
    /// <summary>Assembles the HTTP result from the application result.</summary>
    /// <param name="result">The application result.</param>
    /// <param name="controller">The controller producing the result.</param>
    /// <param name="getUserByIdActionName">Action name for the Location header.</param>
    /// <returns>The HTTP action result.</returns>
    public static ActionResult ToActionResultFromCreateUserResult(
        Result<User, CreateUserError> result,
        ControllerBase controller,
        string getUserByIdActionName) =>
        result switch
        {
            Result<User, CreateUserError>.Success success => controller.CreatedAtAction(
                getUserByIdActionName,
                new { id = success.Value.Id },
                UserResourceFromEntityAssembler.ToResourceFromEntity(success.Value)),
            Result<User, CreateUserError>.Failure failure => failure.Error switch
            {
                CreateUserError.DuplicateEmail => controller.Conflict("Email already registered."),
                CreateUserError.InvalidEmail => controller.BadRequest("Invalid e-mail."),
                _ => controller.Problem(title: "Unexpected server error.", detail: "Unexpected error creating user.", statusCode: 500)
            },
            _ => controller.Problem(title: "Unexpected server error.", detail: "Unexpected error processing request.", statusCode: 500)
        };
}