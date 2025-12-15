using ConsultaCreditos.Application.DTOs;
using ConsultaCreditos.Application.Mapping;
using ConsultaCreditos.Application.Messaging.Contracts;
using ConsultaCreditos.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace ConsultaCreditos.UnitTests.Mapping;

public class CreditoMappingTests
{
    [Fact]
    public void ToIntegracaoMessage_DeveConverterSimplesNacionalCorretamente()
    {
        // Arrange
        var dto = new IntegrarCreditoDto
        {
            NumeroCredito = "123456",
            NumeroNfse = "7891011",
            DataConstituicao = DateTime.Parse("2024-02-25"),
            ValorIssqn = 1500.75m,
            TipoCredito = "ISSQN",
            SimplesNacional = "Sim",
            Aliquota = 5.0m,
            ValorFaturado = 30000.00m,
            ValorDeducao = 5000.00m,
            BaseCalculo = 25000.00m
        };

        // Act
        var message = dto.ToIntegracaoMessage();

        // Assert
        message.Should().NotBeNull();
        message.SimplesNacional.Should().BeTrue();
        message.NumeroCredito.Should().Be(dto.NumeroCredito);
    }

    [Fact]
    public void ToIntegracaoMessage_DeveConverterNaoParaSimplesNacional()
    {
        // Arrange
        var dto = new IntegrarCreditoDto
        {
            SimplesNacional = "Não"
        };

        // Act
        var message = dto.ToIntegracaoMessage();

        // Assert
        message.SimplesNacional.Should().BeFalse();
    }

    [Fact]
    public void ToEntity_DeveConverterMensagemParaEntidade()
    {
        // Arrange
        var message = new CreditoIntegracaoMessage
        {
            NumeroCredito = "123456",
            NumeroNfse = "7891011",
            DataConstituicao = DateTime.Parse("2024-02-25"),
            ValorIssqn = 1500.75m,
            TipoCredito = "ISSQN",
            SimplesNacional = true,
            Aliquota = 5.0m,
            ValorFaturado = 30000.00m,
            ValorDeducao = 5000.00m,
            BaseCalculo = 25000.00m
        };

        // Act
        var entity = message.ToEntity();

        // Assert
        entity.Should().NotBeNull();
        entity.NumeroCredito.Should().Be(message.NumeroCredito);
        entity.SimplesNacional.Should().BeTrue();
    }

    [Fact]
    public void ToResponseDto_DeveConverterEntidadeParaDto()
    {
        // Arrange
        var entity = new Credito
        {
            Id = 1,
            NumeroCredito = "123456",
            NumeroNfse = "7891011",
            DataConstituicao = DateTime.Parse("2024-02-25"),
            ValorIssqn = 1500.75m,
            TipoCredito = "ISSQN",
            SimplesNacional = true,
            Aliquota = 5.0m,
            ValorFaturado = 30000.00m,
            ValorDeducao = 5000.00m,
            BaseCalculo = 25000.00m
        };

        // Act
        var dto = entity.ToResponseDto();

        // Assert
        dto.Should().NotBeNull();
        dto.NumeroCredito.Should().Be(entity.NumeroCredito);
        dto.SimplesNacional.Should().Be("Sim");
    }
}

