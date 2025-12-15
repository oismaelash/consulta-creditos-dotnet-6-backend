namespace ConsultaCreditos.Application.DTOs;

public class AuditoriaResponseDto
{
    public long Id { get; set; }
    public string MessageId { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public string TipoConsulta { get; set; } = string.Empty;
    public string ChaveConsulta { get; set; } = string.Empty;
    public string? CorrelationId { get; set; }
}

