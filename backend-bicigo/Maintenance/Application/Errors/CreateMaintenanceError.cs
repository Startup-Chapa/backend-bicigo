namespace backend_bicigo.Maintenance.Application.Errors;

/// <summary>Errors that can occur when creating a maintenance report.</summary>
public enum CreateMaintenanceError
{
    InvalidReport,
    NotFound,
    InvalidTransition,
    UnexpectedError
}