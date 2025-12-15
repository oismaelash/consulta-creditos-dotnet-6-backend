using System.Text.Json;
using ConsultaCreditos.Application.Mapping;
using ConsultaCreditos.Application.Messaging.Contracts;
using ConsultaCreditos.Application.Ports;
using ConsultaCreditos.Infrastructure.Messaging.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConsultaCreditos.Api.HostedServices;

public class AuditoriaConsumerService : BackgroundService
{
    private readonly IConsumer<string, string> _consumer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AuditoriaConsumerService> _logger;
    private readonly string _topic;

    public AuditoriaConsumerService(
        KafkaConsumerFactory consumerFactory,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<AuditoriaConsumerService> logger)
    {
        _consumer = consumerFactory.CreateConsumer("auditoria-consumer-group");
        _scopeFactory = scopeFactory;
        _logger = logger;
        _topic = configuration["Kafka:Topics:Auditoria"] ?? "consulta-creditos-audit";
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(_topic);
        _logger.LogInformation("Consumer de auditoria iniciado. Aguardando mensagens do tópico {Topic}", _topic);

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
                    _logger.LogError(ex, "Erro ao consumir mensagem de auditoria do Kafka");
                    await Task.Delay(500, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao processar mensagem de auditoria");
                    await Task.Delay(500, stoppingToken);
                }
            }
        }
        finally
        {
            _consumer.Close();
            _logger.LogInformation("Consumer de auditoria finalizado");
        }
    }

    private async Task ProcessarMensagemAsync(ConsumeResult<string, string> result, CancellationToken cancellationToken)
    {
        try
        {
            var message = JsonSerializer.Deserialize<ConsultaAuditMessage>(result.Message.Value);
            if (message == null)
            {
                _logger.LogWarning("Mensagem de auditoria inválida recebida. Offset: {Offset}", result.Offset);
                _consumer.Commit(result);
                return;
            }

            _logger.LogInformation(
                "Processando mensagem de auditoria. TipoConsulta: {TipoConsulta}, ChaveConsulta: {ChaveConsulta}, Offset: {Offset}",
                message.TipoConsulta, message.ChaveConsulta, result.Offset);

            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IAuditoriaRepository>();

            // Persistir auditoria
            var auditoria = message.ToEntity();
            await repository.AdicionarAsync(auditoria, cancellationToken);
            await repository.SalvarAlteracoesAsync(cancellationToken);
            
            _logger.LogInformation(
                "Auditoria persistida com sucesso. TipoConsulta: {TipoConsulta}, ChaveConsulta: {ChaveConsulta}, Offset: {Offset}",
                message.TipoConsulta, message.ChaveConsulta, result.Offset);
            
            _consumer.Commit(result);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Erro ao deserializar mensagem de auditoria. Offset: {Offset}", result.Offset);
            _consumer.Commit(result); // Commit para não reprocessar mensagem inválida
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar mensagem de auditoria. Offset: {Offset}", result.Offset);
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

