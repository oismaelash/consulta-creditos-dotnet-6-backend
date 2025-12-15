using System.Text.Json;
using ConsultaCreditos.Application.Messaging.Contracts;
using ConsultaCreditos.Application.Ports;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsultaCreditos.Infrastructure.Messaging.Kafka;

public class KafkaAuditPublisher : IAuditPublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;
    private readonly ILogger<KafkaAuditPublisher> _logger;
    private bool _disposed = false;

    public KafkaAuditPublisher(
        KafkaProducerFactory producerFactory,
        IConfiguration configuration,
        ILogger<KafkaAuditPublisher> logger)
    {
        _producer = producerFactory.CreateProducer();
        _topic = configuration["Kafka:Topics:Auditoria"] ?? "consulta-creditos-audit";
        _logger = logger;
    }

    public async Task PublishAsync(ConsultaAuditMessage message, CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var kafkaMessage = new Message<string, string>
            {
                Key = message.ChaveConsulta,
                Value = json
            };

            var deliveryResult = await _producer.ProduceAsync(_topic, kafkaMessage, cancellationToken);
            _logger.LogInformation(
                "Mensagem de auditoria publicada no tópico {Topic}. Offset: {Offset}, Partition: {Partition}, Key: {Key}",
                _topic, deliveryResult.Offset, deliveryResult.Partition, message.ChaveConsulta);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Erro ao publicar mensagem de auditoria no Kafka. Key: {Key}", message.ChaveConsulta);
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

