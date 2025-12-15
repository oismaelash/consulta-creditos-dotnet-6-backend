using ConsultaCreditos.Application.DTOs;
using ConsultaCreditos.Application.Mapping;
using ConsultaCreditos.Application.Messaging.Contracts;
using ConsultaCreditos.Application.Ports;

namespace ConsultaCreditos.Application.UseCases;

public class ConsultarCreditosPorNfseUseCase
{
    private readonly ICreditoRepository _repository;
    private readonly IAuditPublisher _auditPublisher;

    public ConsultarCreditosPorNfseUseCase(
        ICreditoRepository repository,
        IAuditPublisher auditPublisher)
    {
        _repository = repository;
        _auditPublisher = auditPublisher;
    }

    public async Task<IEnumerable<CreditoResponseDto>> ExecutarAsync(string numeroNfse, CancellationToken cancellationToken = default)
    {
        var creditos = await _repository.ObterPorNumeroNfseAsync(numeroNfse, cancellationToken);
        
        // Publicar evento de auditoria
        var auditMessage = new ConsultaAuditMessage
        {
            TipoConsulta = "PorNfse",
            ChaveConsulta = numeroNfse
        };
        await _auditPublisher.PublishAsync(auditMessage, cancellationToken);

        return creditos.Select(c => c.ToResponseDto());
    }
}

