using ConsultaCreditos.Application.DTOs;
using ConsultaCreditos.Application.Messaging.Contracts;
using ConsultaCreditos.Domain.Entities;

namespace ConsultaCreditos.Application.Mapping;

public static class CreditoMapping
{
    public static CreditoIntegracaoMessage ToIntegracaoMessage(this IntegrarCreditoDto dto)
    {
        return new CreditoIntegracaoMessage
        {
            NumeroCredito = dto.NumeroCredito,
            NumeroNfse = dto.NumeroNfse,
            DataConstituicao = dto.DataConstituicao,
            ValorIssqn = dto.ValorIssqn,
            TipoCredito = dto.TipoCredito,
            SimplesNacional = ParseSimplesNacional(dto.SimplesNacional),
            Aliquota = dto.Aliquota,
            ValorFaturado = dto.ValorFaturado,
            ValorDeducao = dto.ValorDeducao,
            BaseCalculo = dto.BaseCalculo
        };
    }

    public static Credito ToEntity(this CreditoIntegracaoMessage message)
    {
        return new Credito
        {
            NumeroCredito = message.NumeroCredito,
            NumeroNfse = message.NumeroNfse,
            DataConstituicao = message.DataConstituicao,
            ValorIssqn = message.ValorIssqn,
            TipoCredito = message.TipoCredito,
            SimplesNacional = message.SimplesNacional,
            Aliquota = message.Aliquota,
            ValorFaturado = message.ValorFaturado,
            ValorDeducao = message.ValorDeducao,
            BaseCalculo = message.BaseCalculo
        };
    }

    public static CreditoResponseDto ToResponseDto(this Credito entity)
    {
        return new CreditoResponseDto
        {
            NumeroCredito = entity.NumeroCredito,
            NumeroNfse = entity.NumeroNfse,
            DataConstituicao = entity.DataConstituicao,
            ValorIssqn = entity.ValorIssqn,
            TipoCredito = entity.TipoCredito,
            SimplesNacional = entity.SimplesNacional ? "Sim" : "Não",
            Aliquota = entity.Aliquota,
            ValorFaturado = entity.ValorFaturado,
            ValorDeducao = entity.ValorDeducao,
            BaseCalculo = entity.BaseCalculo
        };
    }

    private static bool ParseSimplesNacional(string value)
    {
        return value.Equals("Sim", StringComparison.OrdinalIgnoreCase);
    }
}

