using backend_bicigo.Maintenance.Application.Errors;
using backend_bicigo.Maintenance.Application.Services;
using backend_bicigo.Maintenance.Domain.Model.Aggregates;
using backend_bicigo.Maintenance.Domain.Model.Commands;
using backend_bicigo.Maintenance.Domain.Repositories;
using backend_bicigo.Shared.Application.Patterns;
using backend_bicigo.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Maintenance.Application.Internal.CommandServices;

/// <summary>
///     Maintenance command service.
/// </summary>
/// <param name="reportRepository">Repository for reports.</param>
/// <param name="unitOfWork">Unit of work.</param>
/// <param name="logger">Logger.</param>
public class MaintenanceCommandService(
    IMaintenanceReportRepository reportRepository,
    IUnitOfWork unitOfWork,
    ILogger<MaintenanceCommandService> logger) : IMaintenanceCommandService
{
    /// <inheritdoc />
    public async Task<Result<MaintenanceReport, CreateMaintenanceError>> Handle(CreateMaintenanceReportCommand command, CancellationToken cancellationToken = default)
    {
        var errors = MaintenanceReport.Validate(command.BikeId, command.ProblemType, command.Description);
        if (errors.Count > 0)
        {
            logger.LogWarning("Invalid maintenance report: {Errors}", string.Join("; ", errors.Select(e => $"{e.Key}={e.Value}")));
            return new Result<MaintenanceReport, CreateMaintenanceError>.Failure(CreateMaintenanceError.InvalidReport);
        }

        try
        {
            var entity = new MaintenanceReport(command);
            // New reports inherit the status of previous reports for the same bike (frontend rule).
            var previous = await reportRepository.FindByBikeIdAsync(command.BikeId.Trim(), cancellationToken);
            var last = previous.OrderByDescending(m => m.Id).FirstOrDefault();
            if (last is not null) entity.UpdateBikeStatus(last.Status);
            await reportRepository.AddAsync(entity, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<MaintenanceReport, CreateMaintenanceError>.Success(entity);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid maintenance report payload");
            return new Result<MaintenanceReport, CreateMaintenanceError>.Failure(CreateMaintenanceError.InvalidReport);
        }
        catch (DbUpdateException ex) when (IsDuplicateKeyViolation(ex))
        {
            logger.LogWarning(ex, "Duplicate report id");
            return new Result<MaintenanceReport, CreateMaintenanceError>.Failure(CreateMaintenanceError.InvalidReport);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error creating maintenance report");
            return new Result<MaintenanceReport, CreateMaintenanceError>.Failure(CreateMaintenanceError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<bool, CreateMaintenanceError>> Handle(UpdateBikeStatusCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var reports = await reportRepository.FindByBikeIdAsync(command.BikeId.Trim(), cancellationToken);
            var list = reports.ToList();
            if (list.Count == 0) return new Result<bool, CreateMaintenanceError>.Failure(CreateMaintenanceError.NotFound);
            foreach (var report in list) report.UpdateBikeStatus(command.Status);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<bool, CreateMaintenanceError>.Success(true);
        }
        catch (ArgumentException)
        {
            return new Result<bool, CreateMaintenanceError>.Failure(CreateMaintenanceError.InvalidReport);
        }
        catch (InvalidOperationException)
        {
            return new Result<bool, CreateMaintenanceError>.Failure(CreateMaintenanceError.InvalidTransition);
        }
        catch (Exception)
        {
            return new Result<bool, CreateMaintenanceError>.Failure(CreateMaintenanceError.UnexpectedError);
        }
    }

    private static bool IsDuplicateKeyViolation(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (!string.Equals(current.GetType().Name, "MySqlException", StringComparison.Ordinal)) continue;
            var numberProperty = current.GetType().GetProperty("Number");
            if (numberProperty?.PropertyType == typeof(int)
                && numberProperty.GetValue(current) is int errorCode
                && errorCode == 1062) return true;
        }
        return false;
    }
}
