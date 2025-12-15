using ConsultaCreditos.Application.DTOs;
using ConsultaCreditos.Application.Mapping;
using ConsultaCreditos.Application.Ports;

namespace ConsultaCreditos.Application.UseCases;

public class ConsultarAuditoriasUseCase
{
    private readonly IAuditoriaRepository _repository;

    public ConsultarAuditoriasUseCase(IAuditoriaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AuditoriaResponseDto>> ExecutarTodasAsync(CancellationToken cancellationToken = default)
    {
        var auditorias = await _repository.ObterTodasAsync(cancellationToken);
        return auditorias.Select(a => a.ToResponseDto());
    }

    public async Task<IEnumerable<AuditoriaResponseDto>> ExecutarPorTipoAsync(string tipoConsulta, CancellationToken cancellationToken = default)
    {
        var auditorias = await _repository.ObterPorTipoAsync(tipoConsulta, cancellationToken);
        return auditorias.Select(a => a.ToResponseDto());
    }

    public async Task<IEnumerable<AuditoriaResponseDto>> ExecutarPorChaveAsync(string chaveConsulta, CancellationToken cancellationToken = default)
    {
        var auditorias = await _repository.ObterPorChaveAsync(chaveConsulta, cancellationToken);
        return auditorias.Select(a => a.ToResponseDto());
    }
}

