namespace backend_bicigo.Trip.Application.Errors;

/// <summary>Errors that can occur when finishing a trip.</summary>
public enum FinishTripError
{
    /// <summary>The trip does not exist.</summary>
    NotFound,

    /// <summary>The trip is not IN_PROGRESS (only IN_PROGRESS -> FINISHED is allowed).</summary>
    InvalidTransition,

    /// <summary>The distance is missing, negative or out of range.</summary>
    InvalidDistance,

    /// <summary>An unexpected error occurred.</summary>
    UnexpectedError
}