using backend_bicigo.Billing.Application.Internal.CommandServices;
using backend_bicigo.Billing.Application.Internal.QueryServices;
using backend_bicigo.Billing.Application.Services;
using backend_bicigo.Billing.Domain.Repositories;
using backend_bicigo.Billing.Infrastructure.Persistence.EFC.Repositories;
using backend_bicigo.Shared.Domain.Repositories;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Configuration;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Repositories;
using backend_bicigo.Shared.Infrastructure.Persistence.EFC.Seed;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database connection (MySQL)
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

// Billing
builder.Services.AddScoped<IBillingRepository, SubscriptionRepository>();
builder.Services.AddScoped<IBillingCommandService, BillingCommandService>();
builder.Services.AddScoped<IBillingQueryService, BillingQueryService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    DatabaseSeeder.Seed(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Scalar API reference
app.MapScalarApiReference();
app.MapControllers();

app.Run();
