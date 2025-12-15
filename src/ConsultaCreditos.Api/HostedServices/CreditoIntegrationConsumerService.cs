using System.Text.Json;
using ConsultaCreditos.Application.Mapping;
using ConsultaCreditos.Application.Messaging.Contracts;
using ConsultaCreditos.Application.Ports;
using ConsultaCreditos.Infrastructure.Messaging.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsultaCreditos.Api.HostedServices;

public class CreditoIntegrationConsumerService : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ICreditoRepository _repository;
    private readonly ILogger<CreditoIntegrationConsumerService> _logger;
    private readonly string _topic;

    public CreditoIntegrationConsumerService(
        KafkaConsumerFactory consumerFactory,
        ICreditoRepository repository,
        IConfiguration configuration,
        ILogger<CreditoIntegrationConsumerService> logger)
    {
        _consumer = consumerFactory.CreateConsumer("credito-integration-consumer-group");
        _repository = repository;
        _logger = logger;
        _topic = configuration["Kafka:Topics:Integracao"] ?? "integrar-credito-constituido-entry";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);
        _logger.LogInformation("Consumer iniciado. Aguardando mensagens do tópico {Topic}", _topic);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(TimeSpan.FromMilliseconds(100));
                    
                    if (result == null)
                    {
                        await Task.Delay(500, stoppingToken);
                        continue;
                    }

                    if (result.IsPartitionEOF)
                    {
                        _logger.LogDebug("Fim da partição alcançado. Continuando...");
                        continue;
                    }

                    await ProcessarMensagemAsync(result, stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Erro ao consumir mensagem do Kafka");
                    await Task.Delay(500, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao processar mensagem");
                    await Task.Delay(500, stoppingToken);
                }
            }
        }
        finally
        {
            _consumer.Close();
            _logger.LogInformation("Consumer finalizado");
        }
    }

    private async Task ProcessarMensagemAsync(ConsumeResult<string, string> result, CancellationToken cancellationToken)
    {
        try
        {
            var message = JsonSerializer.Deserialize<CreditoIntegracaoMessage>(result.Message.Value);
            if (message == null)
            {
                _logger.LogWarning("Mensagem inválida recebida. Offset: {Offset}", result.Offset);
                _consumer.Commit(result);
                return;
            }

            _logger.LogInformation(
                "Processando mensagem. NumeroCredito: {NumeroCredito}, Offset: {Offset}, Partition: {Partition}",
                message.NumeroCredito, result.Offset, result.Partition);

            // Verificar idempotência
            var existe = await _repository.ExistePorNumeroCreditoAsync(message.NumeroCredito, cancellationToken);
            
            if (existe)
            {
                _logger.LogInformation(
                    "Crédito {NumeroCredito} já existe. Processamento idempotente. Offset: {Offset}",
                    message.NumeroCredito, result.Offset);
                _consumer.Commit(result);
                return;
            }

            // Inserir crédito
            var credito = message.ToEntity();
            await _repository.AdicionarAsync(credito, cancellationToken);
            
            try
            {
                await _repository.SalvarAlteracoesAsync(cancellationToken);
                _logger.LogInformation(
                    "Crédito {NumeroCredito} inserido com sucesso. Offset: {Offset}",
                    message.NumeroCredito, result.Offset);
                _consumer.Commit(result);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) when (ex.InnerException?.Message?.Contains("duplicate") == true || 
                                                                          ex.InnerException?.Message?.Contains("unique") == true)
            {
                // Tratamento de idempotência em caso de race condition
                _logger.LogInformation(
                    "Crédito {NumeroCredito} já existe (race condition). Processamento idempotente. Offset: {Offset}",
                    message.NumeroCredito, result.Offset);
                _consumer.Commit(result);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao deserializar mensagem. Offset: {Offset}", result.Offset);
            _consumer.Commit(result); // Commit para não reprocessar mensagem inválida
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar mensagem. Offset: {Offset}", result.Offset);
            // Em produção, considerar dead-letter queue após N tentativas
            // Por enquanto, commit para não travar o consumer
            _consumer.Commit(result);
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}

