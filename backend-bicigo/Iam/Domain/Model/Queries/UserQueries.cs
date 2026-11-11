namespace backend_bicigo.Iam.Domain.Model.Queries;

/// <summary>Query to fetch a user by server id (GET /users/:id).</summary>
/// <param name="Id">User id.</param>
public record GetUserByIdQuery(int Id);

/// <summary>Query to fetch users by e-mail (GET /users?email=), mirror of IamApi.login.</summary>
/// <param name="Email">E-mail filter.</param>
public record GetUsersByEmailQuery(string Email);