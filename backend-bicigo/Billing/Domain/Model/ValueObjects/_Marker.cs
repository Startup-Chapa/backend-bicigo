using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Shared.Domain.Repositories;

namespace backend_bicigo.Billing.Domain.Model.ValueObjects;

/// <summary>Placeholder to keep the ValueObjects folder present for conventions.</summary>
public static class BillingValueObjectMarker
{
    /// <summary>No-op marker.</summary>
    public static void EnsureMarker() => _ = typeof(IUnitOfWork);
}
