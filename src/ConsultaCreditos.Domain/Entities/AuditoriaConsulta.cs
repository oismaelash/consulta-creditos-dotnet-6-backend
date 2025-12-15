namespace ConsultaCreditos.Domain.Entities;

public class AuditoriaConsulta
{
    public long Id { get; set; }
    public string MessageId { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public string TipoConsulta { get; set; } = string.Empty; // "PorNfse" ou "PorCredito"
    public string ChaveConsulta { get; set; } = string.Empty; // numeroNfse ou numeroCredito
    public string? CorrelationId { get; set; }
}

