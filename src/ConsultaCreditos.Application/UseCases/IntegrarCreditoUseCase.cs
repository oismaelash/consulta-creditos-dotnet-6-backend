using ConsultaCreditos.Application.DTOs;
using ConsultaCreditos.Application.Mapping;
using ConsultaCreditos.Application.Ports;
using Microsoft.Extensions.Logging;

namespace ConsultaCreditos.Application.UseCases;

public class IntegrarCreditoUseCase
{
    private readonly IIntegrationPublisher _publisher;
    private readonly ILogger<IntegrarCreditoUseCase> _logger;

    public IntegrarCreditoUseCase(
        IIntegrationPublisher publisher,
        ILogger<IntegrarCreditoUseCase> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    public async Task ExecutarAsync(IEnumerable<IntegrarCreditoDto> creditos, CancellationToken cancellationToken = default)
    {
        foreach (var credito in creditos)
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
    }
}

