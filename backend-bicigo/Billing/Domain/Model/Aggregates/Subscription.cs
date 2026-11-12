using backend_bicigo.Billing.Domain.Model.Commands;
using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Billing.Domain.Model.Aggregates;

/// <summary>
///     Subscription aggregate. Mirrors <c>billing/domain/model/subscription.js</c> and <c>db.json/subscriptions</c>.
/// </summary>
public class Subscription : IAuditableEntity
{
    /// <summary>Protected parameterless constructor for EF Core.</summary>
    protected Subscription()
    {
        PlanType = null!;
        Status = "ACTIVE";
        StartDate = string.Empty;
    }

    /// <summary>Creates a subscription from a command.</summary>
    /// <param name="command">The creation command.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public Subscription(CreateSubscriptionCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        UserId = command.UserId;
        PlanId = command.PlanId;
        PlanType = command.PlanType;
        Price = command.Price;
        StartDate = command.StartDate ?? DateTime.UtcNow.ToString("o");
        EndDate = command.EndDate;
        Status = "ACTIVE";
    }

    /// <summary>Server id.</summary>
    public int Id { get; private set; }

    /// <summary>User id.</summary>
    public int UserId { get; private set; }

    /// <summary>Plan id.</summary>
    public int PlanId { get; private set; }

    /// <summary>PAY_PER_USE | MONTHLY.</summary>
    public string PlanType { get; private set; }

    /// <summary>Paid price.</summary>
    public decimal Price { get; private set; }

    /// <summary>Start date ISO string.</summary>
    public string StartDate { get; private set; }

    /// <summary>End date ISO string (nullable).</summary>
    public string? EndDate { get; private set; }

    /// <summary>Status, defaults ACTIVE.</summary>
    public string Status { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}
