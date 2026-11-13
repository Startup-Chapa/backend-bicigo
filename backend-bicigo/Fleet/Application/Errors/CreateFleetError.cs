namespace backend_bicigo.Fleet.Application.Errors;

/// <summary>Errors that can occur when creating a fleet resource.</summary>
public enum CreateFleetError
{
    /// <summary>Invalid input.</summary>
    InvalidInput,

    /// <summary>Duplicate business code.</summary>
    DuplicateBikeCode,

    /// <summary>An unexpected error occurred.</summary>
    UnexpectedError
}
