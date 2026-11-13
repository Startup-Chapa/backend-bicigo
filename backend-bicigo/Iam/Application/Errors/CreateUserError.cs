namespace backend_bicigo.Iam.Application.Errors;

/// <summary>Errors that can occur when creating a user.</summary>
public enum CreateUserError
{
    /// <summary>The e-mail is invalid.</summary>
    InvalidEmail,

    /// <summary>A user with the same e-mail already exists.</summary>
    DuplicateEmail,

    /// <summary>An unexpected error occurred.</summary>
    UnexpectedError
}