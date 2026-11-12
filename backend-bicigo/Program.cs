using backend_bicigo.Billing.Application.Internal.CommandServices;
using backend_bicigo.Billing.Application.Internal.QueryServices;
using backend_bicigo.Billing.Application.Services;
using backend_bicigo.Fleet.Application.Internal.CommandServices;
using backend_bicigo.Fleet.Application.Internal.QueryServices;
using backend_bicigo.Fleet.Application.Services;
using backend_bicigo.Fleet.Domain.Repositories;
using backend_bicigo.Fleet.Infrastructure.Persistence.EFC.Repositories;
using backend_bicigo.Iam.Application.Internal.CommandServices;
using backend_bicigo.Iam.Application.Internal.QueryServices;
using backend_bicigo.Iam.Application.Services;
using backend_bicigo.Iam.Domain.Repositories;
using backend_bicigo.Iam.Infrastructure.Persistence.EFC.Repositories;
using backend_bicigo.Maintenance.Application.Internal.CommandServices;
using backend_bicigo.Maintenance.Application.Internal.QueryServices;
using backend_bicigo.Maintenance.Application.Services;
using backend_bicigo.Maintenance.Domain.Repositories;
using backend_bicigo.Maintenance.Infrastructure.Persistence.EFC.Repositories;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Seed;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Repositories;
using backend_bicigo.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy( name: "AllowALLPolicy", 
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database connection (MySQL, same as before)
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");
    options.UseMySQL(connectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .EnableDetailedErrors();
});

// Shared
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Iam
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();

// Billing
builder.Services.AddScoped<IBillingCommandService, BillingCommandService>();
builder.Services.AddScoped<IBillingQueryService, BillingQueryService>();

// Fleet
builder.Services.AddScoped<IBicycleRepository, BicycleRepository>();
builder.Services.AddScoped<IFleetCommandService, FleetCommandService>();
builder.Services.AddScoped<IFleetQueryService, FleetQueryService>();

// Trip
builder.Services.AddScoped<backend_bicigo.Trip.Application.Services.ITripCommandService, backend_bicigo.Trip.Application.Internal.CommandServices.TripCommandService>();
builder.Services.AddScoped<backend_bicigo.Trip.Application.Services.ITripQueryService, backend_bicigo.Trip.Application.Internal.QueryServices.TripQueryService>();

// Maintenance
builder.Services.AddScoped<IMaintenanceReportRepository, MaintenanceReportRepository>();
builder.Services.AddScoped<IMaintenanceCommandService, MaintenanceCommandService>();
builder.Services.AddScoped<IMaintenanceQueryService, MaintenanceQueryService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    EnsureNewTables(db);
    DatabaseSeeder.Seed(db);
}

static void EnsureNewTables(AppDbContext db)
{
    // EnsureCreated() does not add tables to an existing database.
    // Create the 7 normalized tables if they do not exist yet.
    db.Database.ExecuteSqlRaw("""
        CREATE TABLE IF NOT EXISTS `plan_features` (`id` int NOT NULL AUTO_INCREMENT, `plan_id` int NOT NULL, `name` varchar(255) NOT NULL, `created_at` datetime NULL, `updated_at` datetime NULL, PRIMARY KEY (`id`), INDEX `ix_plan_features_plan_id_name` (`plan_id`, `name`));
        CREATE TABLE IF NOT EXISTS `user_notifications` (`id` int NOT NULL AUTO_INCREMENT, `user_id` int NOT NULL, `title` varchar(255) NOT NULL, `message` varchar(4000) NOT NULL, `type` varchar(255) NOT NULL, `is_read` tinyint(1) NOT NULL, `created_at` datetime NULL, `updated_at` datetime NULL, PRIMARY KEY (`id`), INDEX `ix_user_notifications_user_id` (`user_id`));
        CREATE TABLE IF NOT EXISTS `bicycle_assignments` (`id` int NOT NULL AUTO_INCREMENT, `bicycle_id` int NOT NULL, `bike_point_id` int NOT NULL, `action` varchar(255) NOT NULL, `assigned_at` varchar(255) NOT NULL, `created_at` datetime NULL, `updated_at` datetime NULL, PRIMARY KEY (`id`), INDEX `ix_bicycle_assignments_bicycle_id` (`bicycle_id`), INDEX `ix_bicycle_assignments_bike_point_id` (`bike_point_id`));
        CREATE TABLE IF NOT EXISTS `trip_users` (`id` int NOT NULL AUTO_INCREMENT, `user_id` varchar(255) NOT NULL, `name` varchar(255) NOT NULL, `email` varchar(255) NOT NULL, `role` varchar(255) NOT NULL, `active` tinyint(1) NOT NULL, `created_at` datetime NULL, `updated_at` datetime NULL, PRIMARY KEY (`id`), UNIQUE INDEX `ix_trip_users_user_id` (`user_id`), INDEX `ix_trip_users_email` (`email`));
        CREATE TABLE IF NOT EXISTS `trip_route_points` (`id` int NOT NULL AUTO_INCREMENT, `trip_id` varchar(255) NOT NULL, `latitude` double NOT NULL, `longitude` double NOT NULL, `sequence` int NOT NULL, `recorded_at` varchar(255) NOT NULL, `created_at` datetime NULL, `updated_at` datetime NULL, PRIMARY KEY (`id`), INDEX `ix_trip_route_points_trip_id` (`trip_id`));
        CREATE TABLE IF NOT EXISTS `trip_photo_evidences` (`id` int NOT NULL AUTO_INCREMENT, `trip_id` varchar(255) NOT NULL, `photo_url` varchar(4000) NOT NULL, `captured_at` varchar(255) NOT NULL, `created_at` datetime NULL, `updated_at` datetime NULL, PRIMARY KEY (`id`), INDEX `ix_trip_photo_evidences_trip_id` (`trip_id`));
        CREATE TABLE IF NOT EXISTS `maintenance_status_histories` (`id` int NOT NULL AUTO_INCREMENT, `maintenance_report_id` int NOT NULL, `bike_id` varchar(255) NOT NULL, `previous_status` varchar(255) NOT NULL, `new_status` varchar(255) NOT NULL, `changed_at` varchar(255) NOT NULL, `created_at` datetime NULL, `updated_at` datetime NULL, PRIMARY KEY (`id`), INDEX `ix_maintenance_status_histories_bike_id` (`bike_id`), INDEX `ix_maintenance_status_histories_maintenance_report_id` (`maintenance_report_id`));
        """);
}

// Configure the HTTP request pipeline.
// OpenAPI is mapped in every environment so Scalar keeps working when the app is
// launched without ASPNETCORE_ENVIRONMENT=Development (e.g. dotnet backend-bicigo.dll).
app.MapOpenApi();

app.UseCors("AllowALLPolicy");

// In Development the frontend calls http://localhost:5267; redirecting to https
// breaks the CORS preflight (OPTIONS) and the browser blocks POST/PUT.
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

// Scalar API reference (kept in all environments to preserve API explorer behaviour)
app.MapScalarApiReference();
app.MapControllers();

app.Run();
