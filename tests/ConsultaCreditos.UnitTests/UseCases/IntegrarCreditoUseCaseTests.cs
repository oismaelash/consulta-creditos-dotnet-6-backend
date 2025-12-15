using ConsultaCreditos.Application.DTOs;
using ConsultaCreditos.Application.Messaging.Contracts;
using ConsultaCreditos.Application.Ports;
using ConsultaCreditos.Application.UseCases;
using FluentAssertions;
using Moq;
using Xunit;

namespace ConsultaCreditos.UnitTests.UseCases;

public class IntegrarCreditoUseCaseTests
{
    private readonly Mock<IIntegrationPublisher> _publisherMock;
    private readonly Mock<Microsoft.Extensions.Logging.ILogger<IntegrarCreditoUseCase>> _loggerMock;
    private readonly IntegrarCreditoUseCase _useCase;

    public IntegrarCreditoUseCaseTests()
    {
        _publisherMock = new Mock<IIntegrationPublisher>();
        _loggerMock = new Mock<Microsoft.Extensions.Logging.ILogger<IntegrarCreditoUseCase>>();
        _useCase = new IntegrarCreditoUseCase(_publisherMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task ExecutarAsync_DevePublicarUmaMensagemPorCredito()
    {
        // Arrange
        var creditos = new List<IntegrarCreditoDto>
        {
            new IntegrarCreditoDto
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
            },
            new IntegrarCreditoDto
            {
                NumeroCredito = "789012",
                NumeroNfse = "7891011",
                DataConstituicao = DateTime.Parse("2024-02-26"),
                ValorIssqn = 1200.50m,
                TipoCredito = "ISSQN",
                SimplesNacional = "Não",
                Aliquota = 4.5m,
                ValorFaturado = 25000.00m,
                ValorDeducao = 4000.00m,
                BaseCalculo = 21000.00m
            }
        };

        // Act
        await _useCase.ExecutarAsync(creditos);

        // Assert
        _publisherMock.Verify(
            p => p.PublishAsync(It.IsAny<CreditoIntegracaoMessage>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task ExecutarAsync_DeveConverterSimplesNacionalCorretamente()
    {
        // Arrange
        var creditos = new List<IntegrarCreditoDto>
        {
            new IntegrarCreditoDto
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
            }
        };

        CreditoIntegracaoMessage? capturedMessage = null;
        _publisherMock
            .Setup(p => p.PublishAsync(It.IsAny<CreditoIntegracaoMessage>(), It.IsAny<CancellationToken>()))
            .Callback<CreditoIntegracaoMessage, CancellationToken>((msg, ct) => capturedMessage = msg);

        // Act
        await _useCase.ExecutarAsync(creditos);

        // Assert
        capturedMessage.Should().NotBeNull();
        capturedMessage!.SimplesNacional.Should().BeTrue();
        capturedMessage.NumeroCredito.Should().Be("123456");
    }
}

