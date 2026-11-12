using backend_bicigo.Maintenance.Domain.Model.Commands;
using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Maintenance.Domain.Model.Aggregates;


public class MaintenanceReport : IAuditableEntity
{
    /// <summary>Valid problem types.</summary>
    public static class ProblemTypes
    {
        /// <summary>Brakes.</summary>
        public const string Brakes = "BRAKES";
        /// <summary>Chain.</summary>
        public const string Chain = "CHAIN";
        /// <summary>Wheel.</summary>
        public const string Wheel = "WHEEL";
        /// <summary>Seat.</summary>
        public const string Seat = "SEAT";
        /// <summary>Lights.</summary>
        public const string Lights = "LIGHTS";
        /// <summary>Other.</summary>
        public const string Other = "OTHER";

        /// <summary>Returns true when the value is a known problem type.</summary>
        /// <param name="value">Candidate value.</param>
        /// <returns>True when valid.</returns>
        public static bool IsValid(string? value) =>
            value?.Trim().ToUpperInvariant() is Brakes or Chain or Wheel or Seat or Lights or Other;
    }

    /// <summary>Valid bike statuses used by maintenance (AVAILABLE|MAINTENANCE).</summary>
    public static class BikeStatuses
    {
        /// <summary>Available.</summary>
        public const string Available = "AVAILABLE";
        /// <summary>Under maintenance.</summary>
        public const string Maintenance = "MAINTENANCE";
    }

    /// <summary>Description limits from the frontend REPORT_LIMITS.</summary>
    public static class Limits
    {
        /// <summary>Maximum bikeId length.</summary>
        public const int BikeIdMax = 64;
        /// <summary>Minimum description length.</summary>
        public const int DescriptionMin = 10;
        /// <summary>Maximum description length.</summary>
        public const int DescriptionMax = 1000;
    }

    /// <summary>Protected parameterless constructor for EF Core.</summary>
    protected MaintenanceReport()
    {
        ReportId = null!;
        BikeId = null!;
        ProblemType = null!;
        Description = null!;
        Status = BikeStatuses.Available;
    }

    /// <summary>Creates a report from a validate command.</summary>
    
    public MaintenanceReport(CreateMaintenanceReportCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        var errors = Validate(command.BikeId, command.ProblemType, command.Description);
        if (errors.Count > 0) throw new ArgumentException(string.Join("; ", errors.Select(e => $"{e.Key}: {e.Value}")));
        ReportId = command.ReportId ?? $"RPT-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
        BikeId = command.BikeId!.Trim();
        ProblemType = command.ProblemType!.Trim().ToUpperInvariant();
        Description = command.Description!.Trim();
        Status = BikeStatuses.Available;
        CreatedAtUtc = DateTime.UtcNow.ToString("o");
    }

    /// <summary>Server-generated id.</summary>
    public int Id { get; private set; }

    /// <summary>Business report id.</summary>
    public string ReportId { get; private set; }

    /// <summary>Bicycle reference (max 64 chars).</summary>
    public string BikeId { get; private set; }

    /// <summary>Problem type.</summary>
    public string ProblemType { get; private set; }

    /// <summary>Description (10-1000 chars).</summary>
    public string Description { get; private set; }

    /// <summary>Bike status AVAILABLE|MAINTENANCE.</summary>
    public string Status { get; private set; }

    /// <summary>Created timestamp ISO string.</summary>
    public string CreatedAtUtc { get; private set; } = string.Empty;

    
    DateTimeOffset? IAuditableEntity.CreatedAt { get; set; }
    
    DateTimeOffset? IAuditableEntity.UpdatedAt { get; set; }

    
    public static Dictionary<string, string> Validate(string? bikeId, string? problemType, string? description)
    {
        var errors = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(bikeId) || bikeId.Trim().Length > Limits.BikeIdMax)
            errors["bikeId"] = "BikeId is required (max 64 chars).";
        if (!ProblemTypes.IsValid(problemType))
            errors["problemType"] = "Invalid problem type.";
        var desc = description?.Trim() ?? string.Empty;
        if (desc.Length < Limits.DescriptionMin || desc.Length > Limits.DescriptionMax)
            errors["description"] = $"Description must be {Limits.DescriptionMin}-{Limits.DescriptionMax} chars.";
        return errors;
    }

    
    public static string? NextBikeStatus(string? status) => status switch
    {
        BikeStatuses.Available => BikeStatuses.Maintenance,
        BikeStatuses.Maintenance => BikeStatuses.Available,
        _ => null
    };

    /// <summary>Toggle the bike status (AVAILABLE ↔ MAINTENANCE).</summary>
    /// <exception cref="InvalidOperationException">Thrown when the current status has no transition.</exception>
    public void ToggleBikeStatus()
    {
        Status = NextBikeStatus(Status) ?? throw new InvalidOperationException($"No transition from status '{Status}'.");
    }

    /// <summary>Update the bike status with transition validation.</summary>
    
    public void UpdateBikeStatus(string status)
    {
        var normalized = (status ?? string.Empty).Trim().ToUpperInvariant();
        if (normalized is not (BikeStatuses.Available or BikeStatuses.Maintenance))
            throw new ArgumentException($"Invalid bike status '{status}'.", nameof(status));
        if (NextBikeStatus(Status) != normalized && Status != normalized)
            throw new InvalidOperationException($"Invalid transition {Status} → {normalized}.");
        Status = normalized;
    }
}
