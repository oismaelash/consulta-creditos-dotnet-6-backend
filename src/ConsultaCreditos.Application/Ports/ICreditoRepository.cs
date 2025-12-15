using ConsultaCreditos.Domain.Entities;

namespace ConsultaCreditos.Application.Ports;

public interface ICreditoRepository
{
    Task<Credito?> ObterPorNumeroCreditoAsync(string numeroCredito, CancellationToken cancellationToken = default);
    Task<IEnumerable<Credito>> ObterPorNumeroNfseAsync(string numeroNfse, CancellationToken cancellationToken = default);
    Task<bool> ExistePorNumeroCreditoAsync(string numeroCredito, CancellationToken cancellationToken = default);
    Task<Credito> AdicionarAsync(Credito credito, CancellationToken cancellationToken = default);
    Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
}

