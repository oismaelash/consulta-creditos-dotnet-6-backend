using ConsultaCreditos.Application.DTOs;
using ConsultaCreditos.Application.Mapping;
using ConsultaCreditos.Application.Ports;

namespace ConsultaCreditos.Application.UseCases;

public class ConsultarAuditoriaPorIdUseCase
{
    private readonly IAuditoriaRepository _repository;

    public ConsultarAuditoriaPorIdUseCase(IAuditoriaRepository repository)
    {
        _repository = repository;
    }

    public async Task<AuditoriaResponseDto?> ExecutarAsync(long id, CancellationToken cancellationToken = default)
    {
        var auditoria = await _repository.ObterPorIdAsync(id, cancellationToken);
        return auditoria?.ToResponseDto();
    }
}

