using System.Reflection;
using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Billing.Domain.Model.Commands;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;

namespace backend_bicigo.Shared.Infrastructure.Persistence.EFC.Seed;

/// <summary>
///     Mock seed data (mirrors server/db.json) loaded on startup when tables are empty.
///     Idempotent: safe to run on every startup.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>Seeds mock data if the tables are empty.</summary>
    /// <param name="context">The database context.</param>
    public static void Seed(AppDbContext context)
    {
        const int demoUserId = 1;

        // Plans
        Plan plan1, plan2;
        if (!context.Set<Plan>().Any())
        {
            plan1 = NewPlan("Plan Tarifa", "PAY_PER_USE", 0m, 0.29m, "Pago por uso por kilometro", "Viajes ilimitados,Cobro por kilometro,Soporte basico", false, false);
            plan2 = NewPlan("Suscripcion Pro", "MONTHLY", 30m, 0m, "Suscripcion mensual premium", "Viajes ilimitados,Sin costo por kilometro,Soporte prioritario", true, false);
            context.Set<Plan>().AddRange(plan1, plan2);
            context.SaveChanges();
        }
        else
        {
            plan1 = context.Set<Plan>().First(p => p.Name == "Plan Tarifa");
            plan2 = context.Set<Plan>().First(p => p.Name == "Suscripcion Pro");
        }

        if (!context.Set<PlanFeature>().Any())
        {
            context.Set<PlanFeature>().AddRange(new PlanFeature(plan1.Id, "Viajes ilimitados"), new PlanFeature(plan1.Id, "Cobro por kilometro"));
        }
        if (!context.Set<Subscription>().Any())
        {
            context.Set<Subscription>().Add(new Subscription(new CreateSubscriptionCommand(demoUserId, plan1.Id, "PAY_PER_USE", 0m, "2026-10-01T10:00:00Z")));
            context.Set<Subscription>().Add(new Subscription(new CreateSubscriptionCommand(demoUserId, plan2.Id, "MONTHLY", 30m, "2026-10-01T10:00:00Z")));
        }
        if (!context.Set<Payment>().Any())
        {
            context.Set<Payment>().Add(new Payment(new CreatePaymentCommand(null, null, demoUserId, "TOPUP", 50m, "PEN", false, "2026-10-01T09:00:00Z")));
            context.Set<Payment>().Add(new Payment(new CreatePaymentCommand(null, "TRIP-001", demoUserId, "CHARGE", 9m, "PEN", true, "2026-10-01T11:00:00Z")));
        }
        if (!context.Set<PaymentMethod>().Any())
        {
            context.Set<PaymentMethod>().Add(new PaymentMethod(new CreatePaymentMethodCommand(demoUserId, "****2777", "CARD", true)));
        }
        context.SaveChanges();
    }

    private static Plan NewPlan(string name, string type, decimal price, decimal pricePerKm, string description, string features, bool recommended, bool current)
    {
        var plan = (Plan)Activator.CreateInstance(typeof(Plan), nonPublic: true)!;
        Set(plan, "Name", name);
        Set(plan, "Type", type);
        Set(plan, "Price", price);
        Set(plan, "PricePerKm", pricePerKm);
        Set(plan, "Currency", "PEN");
        Set(plan, "Description", description);
        Set(plan, "Features", features);
        Set(plan, "Recommended", recommended);
        Set(plan, "Current", current);
        return plan;
    }

    private static void Set(object obj, string prop, object? value)
    {
        var p = obj.GetType().GetProperty(prop, BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
        if (p is null || !p.CanWrite) return;
        try { p.SetValue(obj, value); } catch { /* ignore seed type mismatch */ }
    }
}
