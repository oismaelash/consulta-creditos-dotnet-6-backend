using ConsultaCreditos.Application.Ports;
using ConsultaCreditos.Domain.Entities;
using ConsultaCreditos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConsultaCreditos.Infrastructure.Repositories;

public class AuditoriaRepository : IAuditoriaRepository
{
    private readonly AppDbContext _context;

    public AuditoriaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AuditoriaConsulta>> ObterTodasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AuditoriasConsultas
            .OrderByDescending(a => a.OccurredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditoriaConsulta>> ObterPorTipoAsync(string tipoConsulta, CancellationToken cancellationToken = default)
    {
        return await _context.AuditoriasConsultas
            .Where(a => a.TipoConsulta == tipoConsulta)
            .OrderByDescending(a => a.OccurredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AuditoriaConsulta>> ObterPorChaveAsync(string chaveConsulta, CancellationToken cancellationToken = default)
    {
        return await _context.AuditoriasConsultas
            .Where(a => a.ChaveConsulta == chaveConsulta)
            .OrderByDescending(a => a.OccurredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<AuditoriaConsulta?> ObterPorIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.AuditoriasConsultas
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<AuditoriaConsulta> AdicionarAsync(AuditoriaConsulta auditoria, CancellationToken cancellationToken = default)
    {
        await _context.AuditoriasConsultas.AddAsync(auditoria, cancellationToken);
        return auditoria;
    }

    public async Task<int> SalvarAlteracoesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}

