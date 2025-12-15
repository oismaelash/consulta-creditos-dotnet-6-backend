using ConsultaCreditos.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConsultaCreditos.Infrastructure.Persistence;

public class AuditoriaConsultaEntityTypeConfiguration : IEntityTypeConfiguration<AuditoriaConsulta>
{
    public void Configure(EntityTypeBuilder<AuditoriaConsulta> builder)
    {
        builder.ToTable("auditoria_consulta");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(a => a.MessageId)
            .HasColumnName("message_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.OccurredAt)
            .HasColumnName("occurred_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(a => a.TipoConsulta)
            .HasColumnName("tipo_consulta")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.ChaveConsulta)
            .HasColumnName("chave_consulta")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.CorrelationId)
            .HasColumnName("correlation_id")
            .HasMaxLength(100);

        // Índices para consultas frequentes
        builder.HasIndex(a => a.TipoConsulta);
        builder.HasIndex(a => a.ChaveConsulta);
        builder.HasIndex(a => a.OccurredAt);
    }
}

