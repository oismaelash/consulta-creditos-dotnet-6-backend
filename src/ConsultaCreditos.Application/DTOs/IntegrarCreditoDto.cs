namespace ConsultaCreditos.Application.DTOs;

public class IntegrarCreditoDto
{
    public string NumeroCredito { get; set; } = string.Empty;
    public string NumeroNfse { get; set; } = string.Empty;
    public DateTime DataConstituicao { get; set; }
    public decimal ValorIssqn { get; set; }
    public string TipoCredito { get; set; } = string.Empty;
    public string SimplesNacional { get; set; } = string.Empty; // "Sim" ou "Não"
    public decimal Aliquota { get; set; }
    public decimal ValorFaturado { get; set; }
    public decimal ValorDeducao { get; set; }
    public decimal BaseCalculo { get; set; }
}

