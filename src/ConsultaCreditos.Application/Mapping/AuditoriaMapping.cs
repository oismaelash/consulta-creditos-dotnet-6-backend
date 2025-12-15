using ConsultaCreditos.Application.DTOs;
using ConsultaCreditos.Application.Messaging.Contracts;
using ConsultaCreditos.Domain.Entities;

namespace ConsultaCreditos.Application.Mapping;

public static class AuditoriaMapping
{
    public static AuditoriaConsulta ToEntity(this ConsultaAuditMessage message)
    {
        return new AuditoriaConsulta
        {
            MessageId = message.MessageId,
            OccurredAt = message.OccurredAt,
            TipoConsulta = message.TipoConsulta,
            ChaveConsulta = message.ChaveConsulta,
            CorrelationId = message.CorrelationId
        };
    }

    public static AuditoriaResponseDto ToResponseDto(this AuditoriaConsulta entity)
    {
        return new AuditoriaResponseDto
        {
            Id = entity.Id,
            MessageId = entity.MessageId,
            OccurredAt = entity.OccurredAt,
            TipoConsulta = entity.TipoConsulta,
            ChaveConsulta = entity.ChaveConsulta,
            CorrelationId = entity.CorrelationId
        };
    }
}

