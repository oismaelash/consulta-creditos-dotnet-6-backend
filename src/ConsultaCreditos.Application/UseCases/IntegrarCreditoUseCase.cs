using ConsultaCreditos.Application.DTOs;
using ConsultaCreditos.Application.Mapping;
using ConsultaCreditos.Application.Messaging.Contracts;
using ConsultaCreditos.Application.Ports;
using Microsoft.Extensions.Logging;

namespace ConsultaCreditos.Application.UseCases;

public class IntegrarCreditoUseCase
{
    private readonly IIntegrationPublisher _publisher;
    private readonly IAuditPublisher _auditPublisher;
    private readonly ILogger<IntegrarCreditoUseCase> _logger;

    public IntegrarCreditoUseCase(
        IIntegrationPublisher publisher,
        IAuditPublisher auditPublisher,
        ILogger<IntegrarCreditoUseCase> logger)
    {
        _publisher = publisher;
        _auditPublisher = auditPublisher;
        _logger = logger;
    }

    public async Task ExecutarAsync(IEnumerable<IntegrarCreditoDto> creditos, CancellationToken cancellationToken = default)
    {
        var creditosList = creditos.ToList();
        var quantidadeCreditos = creditosList.Count;
        var chaveAuditoria = $"Integracao-{quantidadeCreditos}-creditos-{DateTime.UtcNow:yyyyMMddHHmmss}";

        foreach (var credito in creditosList)
        {
            try
            {
                var message = credito.ToIntegracaoMessage();
                await _publisher.PublishAsync(message, cancellationToken);
                _logger.LogInformation("Crédito {NumeroCredito} publicado no Kafka", credito.NumeroCredito);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar crédito {NumeroCredito}", credito.NumeroCredito);
                throw;
            }
        }

        // Publicar evento de auditoria após publicar todos os créditos
        try
        {
            var auditMessage = new ConsultaAuditMessage
            {
                TipoConsulta = "IntegracaoCredito",
                ChaveConsulta = chaveAuditoria,
                CorrelationId = string.Join(",", creditosList.Select(c => c.NumeroCredito))
            };
            await _auditPublisher.PublishAsync(auditMessage, cancellationToken);
            _logger.LogInformation("Auditoria de integração publicada. Quantidade: {Quantidade}, Chave: {Chave}", 
                quantidadeCreditos, chaveAuditoria);
        }
        catch (Exception ex)
        {
            // Não falhar a operação principal se a auditoria falhar
            _logger.LogWarning(ex, "Erro ao publicar auditoria de integração. Continuando operação.");
        }
    }
}

