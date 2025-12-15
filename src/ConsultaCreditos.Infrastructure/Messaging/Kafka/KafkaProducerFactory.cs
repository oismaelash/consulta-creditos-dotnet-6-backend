using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsultaCreditos.Infrastructure.Messaging.Kafka;

public class KafkaProducerFactory
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<KafkaProducerFactory> _logger;

    public KafkaProducerFactory(IConfiguration configuration, ILogger<KafkaProducerFactory> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public IProducer<string, string> CreateProducer()
    {
        var bootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true
        };

        var producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((producer, error) =>
            {
                _logger.LogError("Kafka Producer Error: {Reason}", error.Reason);
            })
            .Build();

        return producer;
    }
}

