namespace ConsultaCreditos.Application.Messaging.Contracts;

public class ConsultaAuditMessage
{
    public string MessageId { get; set; } = Guid.NewGuid().ToString();
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public string TipoConsulta { get; set; } = string.Empty; // "PorNfse" ou "PorCredito"
    public string ChaveConsulta { get; set; } = string.Empty; // numeroNfse ou numeroCredito
    public string? CorrelationId { get; set; }
}

