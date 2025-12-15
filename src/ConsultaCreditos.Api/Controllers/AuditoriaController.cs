using ConsultaCreditos.Application.DTOs;
using ConsultaCreditos.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaCreditos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditoriaController : ControllerBase
{
    private readonly ConsultarAuditoriasUseCase _consultarAuditoriasUseCase;
    private readonly ConsultarAuditoriaPorIdUseCase _consultarPorIdUseCase;

    public AuditoriaController(
        ConsultarAuditoriasUseCase consultarAuditoriasUseCase,
        ConsultarAuditoriaPorIdUseCase consultarPorIdUseCase)
    {
        _consultarAuditoriasUseCase = consultarAuditoriasUseCase;
        _consultarPorIdUseCase = consultarPorIdUseCase;
    }

    /// <summary>
    /// Lista todas as auditorias de consultas realizadas
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de auditorias ordenadas por data (mais recentes primeiro)</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AuditoriaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AuditoriaResponseDto>>> ObterTodas(
        CancellationToken cancellationToken)
    {
        var auditorias = await _consultarAuditoriasUseCase.ExecutarTodasAsync(cancellationToken);
        return Ok(auditorias);
    }

    /// <summary>
    /// Obtém uma auditoria específica por ID
    /// </summary>
    /// <param name="id">ID da auditoria</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Detalhes da auditoria</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AuditoriaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AuditoriaResponseDto>> ObterPorId(
        long id,
        CancellationToken cancellationToken)
    {
        var auditoria = await _consultarPorIdUseCase.ExecutarAsync(id, cancellationToken);
        
        if (auditoria == null)
        {
            return NotFound(new { message = $"Auditoria com ID {id} não encontrada" });
        }

        return Ok(auditoria);
    }

    /// <summary>
    /// Lista auditorias filtradas por tipo de consulta (PorNfse ou PorCredito)
    /// </summary>
    /// <param name="tipoConsulta">Tipo de consulta: PorNfse ou PorCredito</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de auditorias do tipo especificado</returns>
    [HttpGet("tipo/{tipoConsulta}")]
    [ProducesResponseType(typeof(IEnumerable<AuditoriaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AuditoriaResponseDto>>> ObterPorTipo(
        string tipoConsulta,
        CancellationToken cancellationToken)
    {
        var auditorias = await _consultarAuditoriasUseCase.ExecutarPorTipoAsync(tipoConsulta, cancellationToken);
        return Ok(auditorias);
    }

    /// <summary>
    /// Lista auditorias filtradas por chave de consulta (número da NFS-e ou número do crédito)
    /// </summary>
    /// <param name="chaveConsulta">Chave de consulta (número da NFS-e ou número do crédito)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de auditorias para a chave especificada</returns>
    [HttpGet("chave/{chaveConsulta}")]
    [ProducesResponseType(typeof(IEnumerable<AuditoriaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AuditoriaResponseDto>>> ObterPorChave(
        string chaveConsulta,
        CancellationToken cancellationToken)
    {
        var auditorias = await _consultarAuditoriasUseCase.ExecutarPorChaveAsync(chaveConsulta, cancellationToken);
        return Ok(auditorias);
    }
}

