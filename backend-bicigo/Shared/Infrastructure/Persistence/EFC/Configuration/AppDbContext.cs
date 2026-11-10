using backend_bicigo.Billing.Domain.Model.Aggregates;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
///     Application database context (BiciGO).
/// </summary>
/// <remarks>
///     Registers the Billing bounded context entities. Table/column names use the
///     snake_case plural convention via
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

        builder.UseSnakeCaseNamingConvention();
    }
}
