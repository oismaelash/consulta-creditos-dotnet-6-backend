using ConsultaCreditos.Application.Messaging.Contracts;

namespace ConsultaCreditos.Application.Ports;

public interface IAuditPublisher
{
    Task PublishAsync(ConsultaAuditMessage message, CancellationToken cancellationToken = default);
}

