using ConsultaCreditos.Application.DTOs;
using ConsultaCreditos.Application.Mapping;
using ConsultaCreditos.Application.Messaging.Contracts;
using ConsultaCreditos.Application.Ports;

namespace ConsultaCreditos.Application.UseCases;

public class ConsultarCreditoPorNumeroUseCase
{
    private readonly ICreditoRepository _repository;
    private readonly IAuditPublisher _auditPublisher;

    public ConsultarCreditoPorNumeroUseCase(
        ICreditoRepository repository,
        IAuditPublisher auditPublisher)
    {
        _repository = repository;
        _auditPublisher = auditPublisher;
    }

    public async Task<CreditoResponseDto?> ExecutarAsync(string numeroCredito, CancellationToken cancellationToken = default)
    {
        var credito = await _repository.ObterPorNumeroCreditoAsync(numeroCredito, cancellationToken);
        
        // Publicar evento de auditoria
        var auditMessage = new ConsultaAuditMessage
        {
            TipoConsulta = "PorCredito",
            ChaveConsulta = numeroCredito
        };
        await _auditPublisher.PublishAsync(auditMessage, cancellationToken);

        return credito?.ToResponseDto();
    }
}

