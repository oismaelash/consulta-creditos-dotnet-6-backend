using ConsultaCreditos.Api.HealthChecks;
using ConsultaCreditos.Api.HostedServices;
using ConsultaCreditos.Application.Ports;
using ConsultaCreditos.Application.UseCases;
using ConsultaCreditos.Infrastructure.Messaging.Kafka;
using ConsultaCreditos.Infrastructure.Persistence;
using ConsultaCreditos.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
var connectionString = builder.Configuration.GetConnectionString("Postgres") 
    ?? "Host=localhost;Port=5432;Database=consultacreditos;Username=postgres;Password=postgres";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repositories
builder.Services.AddScoped<ICreditoRepository, CreditoRepository>();

// Kafka Factories
builder.Services.AddSingleton<KafkaProducerFactory>();
builder.Services.AddSingleton<KafkaConsumerFactory>();

// Publishers
builder.Services.AddSingleton<IIntegrationPublisher, KafkaIntegrationPublisher>();
builder.Services.AddSingleton<IAuditPublisher, KafkaAuditPublisher>();

// Use Cases
builder.Services.AddScoped<IntegrarCreditoUseCase>();
builder.Services.AddScoped<ConsultarCreditosPorNfseUseCase>();
builder.Services.AddScoped<ConsultarCreditoPorNumeroUseCase>();

// Background Service
builder.Services.AddHostedService<CreditoIntegrationConsumerService>();

// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck<PostgresHealthCheck>("postgres", tags: new[] { "ready" })
    .AddCheck<KafkaHealthCheck>("kafka", tags: new[] { "ready" })
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "self" });

var app = builder.Build();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
