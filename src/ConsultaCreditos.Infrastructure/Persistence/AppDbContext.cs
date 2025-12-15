using ConsultaCreditos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConsultaCreditos.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Credito> Creditos { get; set; } = null!;
    public DbSet<AuditoriaConsulta> AuditoriasConsultas { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}

