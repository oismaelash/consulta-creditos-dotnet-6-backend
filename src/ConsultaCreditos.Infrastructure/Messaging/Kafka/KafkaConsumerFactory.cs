using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsultaCreditos.Infrastructure.Messaging.Kafka;

public class KafkaConsumerFactory
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<KafkaConsumerFactory> _logger;

    public KafkaConsumerFactory(IConfiguration configuration, ILogger<KafkaConsumerFactory> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public IConsumer<string, string> CreateConsumer(string groupId)
    {
        var bootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
        
        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false, // Commit manual após processamento
            EnablePartitionEof = true
        };

        var consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((consumer, error) =>
            {
                _logger.LogError("Kafka Consumer Error: {Reason}", error.Reason);
            })
            .Build();

        return consumer;
    }
}

