using ConsultaCreditos.Application.Messaging.Contracts;

namespace ConsultaCreditos.Application.Ports;

public interface IIntegrationPublisher
{
    Task PublishAsync(CreditoIntegracaoMessage message, CancellationToken cancellationToken = default);
}

