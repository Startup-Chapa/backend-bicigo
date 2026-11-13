using backend_bicigo.Billing.Domain.Model.Commands;
using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Billing.Domain.Model.Aggregates;

/// <summary>
///     Payment aggregate. Mirrors <c>billing/domain/model/payment.js</c> and <c>db.json/payments</c>.
/// </summary>
/// <remarks>
///     type: null | CHARGE | TOPUP; isAutomatic: true for trip charges.
/// </remarks>
public class Payment : IAuditableEntity
{
    /// <summary>Protected parameterless constructor for EF Core.</summary>
    protected Payment()
    {
        Currency = "PEN";
        Status = "COMPLETED";
        CreatedAt = string.Empty;
    }

    /// <summary>Creates a payment from a command.</summary>
    /// <param name="command">The creation command.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public Payment(CreatePaymentCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        SubscriptionId = command.SubscriptionId;
        TripId = command.TripId;
        UserId = command.UserId;
        Type = command.Type;
        Amount = command.Amount;
        Currency = command.Currency ?? "PEN";
        Status = "COMPLETED";
        IsAutomatic = command.IsAutomatic;
        CreatedAt = command.CreatedAt ?? DateTime.UtcNow.ToString("o");
    }

    /// <summary>Server id.</summary>
    public int Id { get; private set; }

    /// <summary>Subscription id (nullable).</summary>
    public int? SubscriptionId { get; private set; }

    /// <summary>Trip id (nullable).</summary>
    public string? TripId { get; private set; }

    /// <summary>User id (nullable).</summary>
    public int? UserId { get; private set; }

    /// <summary>CHARGE | TOPUP | null.</summary>
    public string? Type { get; private set; }

    /// <summary>Amount in PEN.</summary>
    public decimal Amount { get; private set; }

    /// <summary>Currency PEN.</summary>
    public string Currency { get; private set; }

    /// <summary>Status COMPLETED.</summary>
    public string Status { get; private set; }

    /// <summary>Whether it was generated automatically (trip charge).</summary>
    public bool IsAutomatic { get; private set; }

    /// <summary>Created timestamp ISO string.</summary>
    public string CreatedAt { get; private set; }

    // IAuditableEntity stamps (CreatedAt prop name collides with the string CreatedAt field above).
    // Keep audit fields separate via explicit names to avoid EF confusion.
    /// <inheritdoc />
    DateTimeOffset? IAuditableEntity.CreatedAt { get; set; }

    /// <inheritdoc />
    DateTimeOffset? IAuditableEntity.UpdatedAt { get; set; }
}
