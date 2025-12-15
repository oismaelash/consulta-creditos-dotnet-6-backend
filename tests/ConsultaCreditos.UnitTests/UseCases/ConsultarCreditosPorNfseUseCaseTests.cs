using ConsultaCreditos.Application.DTOs;
using ConsultaCreditos.Application.Messaging.Contracts;
using ConsultaCreditos.Application.Ports;
using ConsultaCreditos.Application.UseCases;
using ConsultaCreditos.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace ConsultaCreditos.UnitTests.UseCases;

public class ConsultarCreditosPorNfseUseCaseTests
{
    private readonly Mock<ICreditoRepository> _repositoryMock;
    private readonly Mock<IAuditPublisher> _auditPublisherMock;
    private readonly ConsultarCreditosPorNfseUseCase _useCase;

    public ConsultarCreditosPorNfseUseCaseTests()
    {
        _repositoryMock = new Mock<ICreditoRepository>();
        _auditPublisherMock = new Mock<IAuditPublisher>();
        _useCase = new ConsultarCreditosPorNfseUseCase(_repositoryMock.Object, _auditPublisherMock.Object);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarCreditosEmitidos()
    {
        // Arrange
        var numeroNfse = "7891011";
        var creditos = new List<Credito>
        {
            new Credito
            {
                Id = 1,
                NumeroCredito = "123456",
                NumeroNfse = numeroNfse,
                DataConstituicao = DateTime.Parse("2024-02-25"),
                ValorIssqn = 1500.75m,
                TipoCredito = "ISSQN",
                SimplesNacional = true,
                Aliquota = 5.0m,
                ValorFaturado = 30000.00m,
                ValorDeducao = 5000.00m,
                BaseCalculo = 25000.00m
            }
        };

        _repositoryMock
            .Setup(r => r.ObterPorNumeroNfseAsync(numeroNfse, It.IsAny<CancellationToken>()))
            .ReturnsAsync(creditos);

        // Act
        var result = await _useCase.ExecutarAsync(numeroNfse);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().NumeroCredito.Should().Be("123456");
        result.First().SimplesNacional.Should().Be("Sim");

        _auditPublisherMock.Verify(
            p => p.PublishAsync(
                It.Is<ConsultaAuditMessage>(m => m.TipoConsulta == "PorNfse" && m.ChaveConsulta == numeroNfse),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

