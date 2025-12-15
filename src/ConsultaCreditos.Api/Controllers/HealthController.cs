using System.Linq;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ConsultaCreditos.Api.Controllers;

[ApiController]
[Route("")]
public class HealthController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    /// <summary>
    /// Verifica se o processo está de pé (liveness)
    /// </summary>
    [HttpGet("self")]
    [ProducesResponseType(typeof(HealthCheckResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> Self()
    {
        var result = await _healthCheckService.CheckHealthAsync(check => check.Tags.Contains("self"));
        return result.Status == HealthStatus.Healthy 
            ? Ok(new { status = "Healthy", message = "Service is running" })
            : StatusCode(503, new { status = result.Status.ToString(), message = "Service is not healthy" });
    }

    /// <summary>
    /// Verifica se as dependências críticas estão disponíveis (readiness)
    /// </summary>
    [HttpGet("ready")]
    [ProducesResponseType(typeof(HealthCheckResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Ready()
    {
        var result = await _healthCheckService.CheckHealthAsync(check => check.Tags.Contains("ready"));
        
        var response = new
        {
            status = result.Status.ToString(),
            checks = result.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                exception = e.Value.Exception?.Message
            })
        };

        return result.Status == HealthStatus.Healthy 
            ? Ok(response)
            : StatusCode(503, response);
    }
}

