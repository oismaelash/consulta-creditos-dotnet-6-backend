using System.Text.Json;
using ConsultaCreditos.Application.Messaging.Contracts;
using ConsultaCreditos.Application.Ports;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsultaCreditos.Infrastructure.Messaging.Kafka;

public class KafkaIntegrationPublisher : IIntegrationPublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;
    private readonly ILogger<KafkaIntegrationPublisher> _logger;
    private bool _disposed = false;

    public KafkaIntegrationPublisher(
        KafkaProducerFactory producerFactory,
        IConfiguration configuration,
        ILogger<KafkaIntegrationPublisher> logger)
    {
        _producer = producerFactory.CreateProducer();
        _topic = configuration["Kafka:Topics:Integracao"] ?? "integrar-credito-constituido-entry";
        _logger = logger;
    }

    public async Task PublishAsync(CreditoIntegracaoMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var kafkaMessage = new Message<string, string>
            {
                Key = message.NumeroCredito,
                Value = json
            };

            var deliveryResult = await _producer.ProduceAsync(_topic, kafkaMessage, cancellationToken);
            _logger.LogInformation(
                "Mensagem publicada no tópico {Topic}. Offset: {Offset}, Partition: {Partition}, Key: {Key}",
                _topic, deliveryResult.Offset, deliveryResult.Partition, message.NumeroCredito);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem no Kafka. Key: {Key}", message.NumeroCredito);
            throw;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _producer?.Flush(TimeSpan.FromSeconds(5));
            _producer?.Dispose();
            _disposed = true;
        }
    }
}

