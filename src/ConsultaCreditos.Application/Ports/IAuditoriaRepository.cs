using ConsultaCreditos.Domain.Entities;

namespace ConsultaCreditos.Application.Ports;

public interface IAuditoriaRepository
{
    Task<IEnumerable<AuditoriaConsulta>> ObterTodasAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditoriaConsulta>> ObterPorTipoAsync(string tipoConsulta, CancellationToken cancellationToken = default);
    Task<IEnumerable<AuditoriaConsulta>> ObterPorChaveAsync(string chaveConsulta, CancellationToken cancellationToken = default);
    Task<AuditoriaConsulta?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default);
    Task<AuditoriaConsulta> AdicionarAsync(AuditoriaConsulta auditoria, CancellationToken cancellationToken = default);
    Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}

