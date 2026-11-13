using System.Reflection;
using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Billing.Domain.Model.Commands;
using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Fleet.Domain.Model.Commands;
using backend_bicigo.Iam.Domain.Model.Aggregates;
using backend_bicigo.Iam.Domain.Model.Commands;
using backend_bicigo.Maintenance.Domain.Model.Aggregates;
using backend_bicigo.Maintenance.Domain.Model.Commands;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Trip.Domain.Model.Aggregates;
using backend_bicigo.Trip.Domain.Model.Commands;

namespace backend_bicigo.Shared.Infrastructure.Persistence.EFC.Seed;

/// <summary>
///     Mock seed data (mirrors server/db.json) loaded on startup when tables are empty.
///     Visible in Scalar (GET endpoints) and in MySQL.
///     Idempotent: safe to run on every startup.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>Seeds mock data if the tables are empty.</summary>
    /// <param name="context">The database context.</param>
    public static void Seed(AppDbContext context)
    {
        // Users
        User serUser1, serUser2;
        if (!context.Set<User>().Any())
        {
            serUser1 = new User(new RegisterUserCommand("usuario@bicigo.test", "BiciGO123!", "Usuario Demo", "", "tarifa", "Lima"));
            serUser1.ApplyUpdate(new UpdateUserCommand(Balance: 49.97m, Role: "USER"));
            serUser2 = new User(new RegisterUserCommand("operador@bicigo.test", "BiciGO123!", "Operador Demo", "", "tarifa", "Lima"));
            serUser2.ApplyUpdate(new UpdateUserCommand(Balance: 0m, Role: "OPERATOR"));
            context.Set<User>().AddRange(serUser1, serUser2);
            context.SaveChanges();
        }
        else
        {
            serUser1 = context.Set<User>().First(u => u.Email == "usuario@bicigo.test");
            serUser2 = context.Set<User>().First(u => u.Email == "operador@bicigo.test");
        }

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
            context.Set<Subscription>().Add(new Subscription(new CreateSubscriptionCommand(serUser1.Id, plan1.Id, "PAY_PER_USE", 0m, "2026-10-01T10:00:00Z")));
            context.Set<Subscription>().Add(new Subscription(new CreateSubscriptionCommand(serUser1.Id, plan2.Id, "MONTHLY", 30m, "2026-10-01T10:00:00Z")));
        }
        if (!context.Set<Payment>().Any())
        {
            context.Set<Payment>().Add(new Payment(new CreatePaymentCommand(null, null, serUser1.Id, "TOPUP", 50m, "PEN", false, "2026-10-01T09:00:00Z")));
            context.Set<Payment>().Add(new Payment(new CreatePaymentCommand(null, "TRIP-001", serUser1.Id, "CHARGE", 9m, "PEN", true, "2026-10-01T11:00:00Z")));
        }
        if (!context.Set<PaymentMethod>().Any())
        {
            context.Set<PaymentMethod>().Add(new PaymentMethod(new CreatePaymentMethodCommand(serUser1.Id, "****2777", "CARD", true)));
        }
        context.SaveChanges();

        BikePoint point;
        if (!context.Set<BikePoint>().Any())
        {
            point = new BikePoint(new RegisterBikePointCommand("Paleta Storm", "Lima", "Jr. Jose Nicolas Rodrigo 100, Miraflores", -12.12, -77.03, 10));
            context.Set<BikePoint>().Add(point);
            context.SaveChanges();
        }
        else
        {
            point = context.Set<BikePoint>().First();
        }

        Bicycle bike1, bike2;
        if (!context.Set<Bicycle>().Any())
        {
            bike1 = new Bicycle(new RegisterBicycleCommand("BG-1042", "BGO-1042", "BICIGO-BGO-1042", "City", point.Id));
            bike2 = new Bicycle(new RegisterBicycleCommand("BG-1043", "BGO-1043", "BICIGO-BGO-1043", "City", point.Id));
            context.Set<Bicycle>().AddRange(bike1, bike2);
            context.Set<BicycleAssignment>().Add(new BicycleAssignment(bike1.Id, point.Id, "ASSIGN"));
            context.SaveChanges();
        }
        else
        {
            bike1 = context.Set<Bicycle>().First(b => b.BikeCode == "BGO-1042");
            bike2 = context.Set<Bicycle>().First(b => b.BikeCode == "BGO-1043");
        }

        if (!context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().Any())
        {
            var trip = new backend_bicigo.Trip.Domain.Model.Aggregates.Trip(new CreateTripCommand("TRIP-001", "USER-001", "BIKE-001", "2026-10-01T10:00:00Z"));
            trip.StartTrip("2026-10-01T10:00:00Z");
            trip.FinishTrip(4.5m, 0.29m, false, "2026-10-01T11:00:00Z");
            context.Set<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().Add(trip);
        }
        if (!context.Set<QrValidation>().Any())
        {
            var qr = new QrValidation(new CreateQrValidationCommand("QRV-001", "TRIP-001", "BIKE-001", "BIKE-001-QR"));
            qr.Validate("BIKE-001-QR");
            context.Set<QrValidation>().Add(qr);
        }
        if (!context.Set<TripUser>().Any())
        {
            context.Set<TripUser>().Add(new TripUser("USER-001", "Usuario Demo 1", "usuario@bicigo.test", "USER", true));
        }
        if (!context.Set<TripRoutePoint>().Any())
        {
            context.Set<TripRoutePoint>().AddRange(
                new TripRoutePoint("TRIP-001", -12.12, -77.03, 0, "2026-10-01T10:05:00Z"),
                new TripRoutePoint("TRIP-001", -12.11, -77.02, 1, "2026-10-01T10:30:00Z"),
                new TripRoutePoint("TRIP-001", -12.105, -77.015, 2, "2026-10-01T11:00:00Z"));
        }
        if (!context.Set<TripPhotoEvidence>().Any())
        {
            context.Set<TripPhotoEvidence>().Add(new TripPhotoEvidence("TRIP-001", "https://example.com/foto-devolucion-001.jpg", "2026-10-01T11:00:00Z"));
        }
        context.SaveChanges();

        MaintenanceReport report;
        if (!context.Set<MaintenanceReport>().Any())
        {
            report = new MaintenanceReport(new CreateMaintenanceReportCommand("BIKE-001", "BRAKES", "Frenos delanteros no responden al presionar la manija", "RPT-001"));
            report.UpdateBikeStatus("MAINTENANCE");
            context.Set<MaintenanceReport>().Add(report);
            context.SaveChanges();
        }
        else
        {
            report = context.Set<MaintenanceReport>().First();
        }
        if (!context.Set<MaintenanceStatusHistory>().Any())
        {
            context.Set<MaintenanceStatusHistory>().Add(new MaintenanceStatusHistory(report.Id, "BIKE-001", "AVAILABLE", "MAINTENANCE", "2026-10-02T09:00:00Z"));
        }
        if (!context.Set<UserNotification>().Any())
        {
            context.Set<UserNotification>().Add(new UserNotification(serUser1.Id, "Bienvenido a BiciGO", "Tu saldo actual es S/ 49.97", "INFO", false));
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
