using backend_bicigo.Billing.Domain.Model.Commands;
using backend_bicigo.Shared.Domain.Model;

namespace backend_bicigo.Billing.Domain.Model.Aggregates;

/// <summary>
///     Payment method aggregate. Mirrors <c>db.json/paymentMethods</c>.
/// </summary>
public class PaymentMethod : IAuditableEntity
{
    /// <summary>Protected parameterless constructor for EF Core.</summary>
    protected PaymentMethod()
    {
        Type = "CARD";
        Token = null!;
    }

    /// <summary>Creates a payment method from a command.</summary>
    /// <param name="command">The creation command.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public PaymentMethod(CreatePaymentMethodCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        UserId = command.UserId;
        Type = command.Type ?? "CARD";
        Token = command.Token;
        IsDefault = command.IsDefault;
    }

    /// <summary>Server id.</summary>
    public int Id { get; private set; }

    /// <summary>User id.</summary>
    public int UserId { get; private set; }

    /// <summary>Method type, e.g. CARD.</summary>
    public string Type { get; private set; }

    /// <summary>Masked token, e.g. ****2777.</summary>
    public string Token { get; private set; }

    /// <summary>Whether it is the default method.</summary>
    public bool IsDefault { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset? CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }
}
