using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Fleet.Domain.Model.Aggregates;
using backend_bicigo.Iam.Domain.Model.Aggregates;
using backend_bicigo.Maintenance.Domain.Model.Aggregates;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Interceptors;
using backend_bicigo.Trip.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
///     Application database context (BiciGO).
/// </summary>
/// <remarks>
///     Registers all 17 entities from Iam, Billing, Fleet, Trip and Maintenance.
///     The original CatchUp FavoriteSource entity was removed (not part of BiciGO).
///     Table/column names use the snake_case plural convention via
///     <see cref="ModelBuilderExtensions.UseSnakeCaseNamingConvention"/>.
/// </remarks>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        // Apply audit timestamp interceptor for all IAuditableEntity implementations
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Iam (2 tables)
        builder.Entity<User>().HasKey(u => u.Id);
        builder.Entity<User>().Property(u => u.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<User>().HasIndex(u => u.Email).IsUnique();

        builder.Entity<UserNotification>().HasKey(u => u.Id);
        builder.Entity<UserNotification>().Property(u => u.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<UserNotification>().HasIndex(u => u.UserId);

        // Billing (5 tables)
        builder.Entity<Plan>().HasKey(p => p.Id);
        builder.Entity<Plan>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();

        builder.Entity<PlanFeature>().HasKey(p => p.Id);
        builder.Entity<PlanFeature>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<PlanFeature>().HasIndex(p => new { p.PlanId, p.Name }).IsUnique();

        builder.Entity<Subscription>().HasKey(s => s.Id);
        builder.Entity<Subscription>().Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Subscription>().HasIndex(s => new { s.UserId, s.Status });

        builder.Entity<Payment>().HasKey(p => p.Id);
        builder.Entity<Payment>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Payment>().HasIndex(p => p.UserId);
        builder.Entity<Payment>().HasIndex(p => p.SubscriptionId);

        builder.Entity<PaymentMethod>().HasKey(p => p.Id);
        builder.Entity<PaymentMethod>().Property(p => p.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<PaymentMethod>().HasIndex(p => p.UserId);

        // Fleet (3 tables)
        builder.Entity<Bicycle>().HasKey(b => b.Id);
        builder.Entity<Bicycle>().Property(b => b.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Bicycle>().HasIndex(b => b.BikeCode).IsUnique();
        builder.Entity<Bicycle>().HasIndex(b => b.SerialNumber).IsUnique();
        builder.Entity<Bicycle>().HasIndex(b => b.CurrentBikePointId);

        builder.Entity<BikePoint>().HasKey(b => b.Id);
        builder.Entity<BikePoint>().Property(b => b.Id).IsRequired().ValueGeneratedOnAdd();

        builder.Entity<BicycleAssignment>().HasKey(b => b.Id);
        builder.Entity<BicycleAssignment>().Property(b => b.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<BicycleAssignment>().HasIndex(b => b.BicycleId);
        builder.Entity<BicycleAssignment>().HasIndex(b => b.BikePointId);

        // Trip (5 tables)
        builder.Entity<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().HasKey(t => t.Id);
        builder.Entity<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().Property(t => t.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().HasIndex(t => t.TripId).IsUnique();
        builder.Entity<backend_bicigo.Trip.Domain.Model.Aggregates.Trip>().HasIndex(t => t.UserId);

        builder.Entity<QrValidation>().HasKey(q => q.Id);
        builder.Entity<QrValidation>().Property(q => q.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<QrValidation>().HasIndex(q => q.ValidationId).IsUnique();

        builder.Entity<TripUser>().HasKey(t => t.Id);
        builder.Entity<TripUser>().Property(t => t.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<TripUser>().HasIndex(t => t.UserId).IsUnique();
        builder.Entity<TripUser>().HasIndex(t => t.Email).IsUnique();

        builder.Entity<TripRoutePoint>().HasKey(t => t.Id);
        builder.Entity<TripRoutePoint>().Property(t => t.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<TripRoutePoint>().HasIndex(t => t.TripId);

        builder.Entity<TripPhotoEvidence>().HasKey(t => t.Id);
        builder.Entity<TripPhotoEvidence>().Property(t => t.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<TripPhotoEvidence>().HasIndex(t => t.TripId);

        // Maintenance (2 tables)
        builder.Entity<MaintenanceReport>().HasKey(m => m.Id);
        builder.Entity<MaintenanceReport>().Property(m => m.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<MaintenanceReport>().HasIndex(m => m.ReportId).IsUnique();
        builder.Entity<MaintenanceReport>().HasIndex(m => m.BikeId);

        builder.Entity<MaintenanceStatusHistory>().HasKey(m => m.Id);
        builder.Entity<MaintenanceStatusHistory>().Property(m => m.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<MaintenanceStatusHistory>().HasIndex(m => m.BikeId);
        builder.Entity<MaintenanceStatusHistory>().HasIndex(m => m.MaintenanceReportId);

        builder.UseSnakeCaseNamingConvention();
    }
}
