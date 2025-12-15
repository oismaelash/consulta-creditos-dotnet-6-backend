using ConsultaCreditos.Application.DTOs;
using ConsultaCreditos.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace ConsultaCreditos.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CreditosController : ControllerBase
{
    private readonly IntegrarCreditoUseCase _integrarCreditoUseCase;
    private readonly ConsultarCreditosPorNfseUseCase _consultarPorNfseUseCase;
    private readonly ConsultarCreditoPorNumeroUseCase _consultarPorNumeroUseCase;

    public CreditosController(
        IntegrarCreditoUseCase integrarCreditoUseCase,
        ConsultarCreditosPorNfseUseCase consultarPorNfseUseCase,
        ConsultarCreditoPorNumeroUseCase consultarPorNumeroUseCase)
    {
        _integrarCreditoUseCase = integrarCreditoUseCase;
        _consultarPorNfseUseCase = consultarPorNfseUseCase;
        _consultarPorNumeroUseCase = consultarPorNumeroUseCase;
    }

    [HttpPost("integrar-credito-constituido")]
    public async Task<IActionResult> IntegrarCreditoConstituido(
        [FromBody] List<IntegrarCreditoDto> creditos,
        CancellationToken cancellationToken)
    {
        if (creditos == null || !creditos.Any())
        {
            return BadRequest(new { success = false, message = "Lista de créditos não pode estar vazia" });
        }

        await _integrarCreditoUseCase.ExecutarAsync(creditos, cancellationToken);

        return Accepted(new { success = true });
    }

    [HttpGet("{numeroNfse}")]
    public async Task<ActionResult<IEnumerable<CreditoResponseDto>>> ObterPorNfse(
        string numeroNfse,
        CancellationToken cancellationToken)
    {
        var creditos = await _consultarPorNfseUseCase.ExecutarAsync(numeroNfse, cancellationToken);
        return Ok(creditos);
    }

    [HttpGet("credito/{numeroCredito}")]
    public async Task<ActionResult<CreditoResponseDto>> ObterPorNumeroCredito(
        string numeroCredito,
        CancellationToken cancellationToken)
    {
        var credito = await _consultarPorNumeroUseCase.ExecutarAsync(numeroCredito, cancellationToken);
        
        if (credito == null)
        {
            return NotFound();
        }

        return Ok(credito);
    }
}

