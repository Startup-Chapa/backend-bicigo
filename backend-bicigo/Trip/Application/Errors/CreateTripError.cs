namespace backend_bicigo.Trip.Application.Errors;

/// <summary>Errors that can occur when creating a trip resource.</summary>
public enum CreateTripError
{
    /// <summary>Invalid input.</summary>
    InvalidInput,

    /// <summary>Duplicate business id.</summary>
    DuplicateTripId,

    /// <summary>An unexpected error occurred.</summary>
    UnexpectedError,

    /// <summary>The user already has an active trip.</summary>
    ActiveTripExists,

    /// <summary>The requested bicycle is not available.</summary>
    BicycleUnavailable,

    /// <summary>The referenced trip or bicycle does not exist.</summary>
    RelatedResourceNotFound
}
