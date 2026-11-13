namespace backend_bicigo.Billing.Application.Errors;

/// <summary>Errors that can occur when creating a billing resource.</summary>
public enum CreateBillingError
{
    /// <summary>Invalid payload.</summary>
    InvalidInput,

    /// <summary>An unexpected error occurred.</summary>
    UnexpectedError
}
