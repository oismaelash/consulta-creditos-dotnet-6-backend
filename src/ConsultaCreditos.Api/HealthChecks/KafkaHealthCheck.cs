using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ConsultaCreditos.Api.HealthChecks;

public class KafkaHealthCheck : IHealthCheck
{
    private readonly IConfiguration _configuration;

    public KafkaHealthCheck(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
            
            var config = new AdminClientConfig
            {
                BootstrapServers = bootstrapServers
            };

            using var adminClient = new AdminClientBuilder(config).Build();
            
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
            
            if (metadata == null || metadata.Brokers.Count == 0)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Kafka não está disponível"));
            }

            return Task.FromResult(HealthCheckResult.Healthy($"Kafka está disponível. Brokers: {metadata.Brokers.Count}"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("Erro ao verificar Kafka", ex));
        }
    }
}

