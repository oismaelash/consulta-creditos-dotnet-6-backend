using ConsultaCreditos.Application.Ports;
using ConsultaCreditos.Domain.Entities;
using ConsultaCreditos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConsultaCreditos.Infrastructure.Repositories;

public class CreditoRepository : ICreditoRepository
{
    private readonly AppDbContext _context;

    public CreditoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Credito?> ObterPorNumeroCreditoAsync(string numeroCredito, CancellationToken cancellationToken = default)
    {
        return await _context.Creditos
            .FirstOrDefaultAsync(c => c.NumeroCredito == numeroCredito, cancellationToken);
    }

    public async Task<IEnumerable<Credito>> ObterPorNumeroNfseAsync(string numeroNfse, CancellationToken cancellationToken = default)
    {
        return await _context.Creditos
            .Where(c => c.NumeroNfse == numeroNfse)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistePorNumeroCreditoAsync(string numeroCredito, CancellationToken cancellationToken = default)
    {
        return await _context.Creditos
            .AnyAsync(c => c.NumeroCredito == numeroCredito, cancellationToken);
    }

    public async Task<Credito> AdicionarAsync(Credito credito, CancellationToken cancellationToken = default)
    {
        await _context.Creditos.AddAsync(credito, cancellationToken);
        return credito;
    }

    public async Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}

