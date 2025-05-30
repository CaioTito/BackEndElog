using BackEndElog.Api.Extensions;
using BackEndElog.Application.Queries;
using BackEndElog.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BackEndElog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OdometerController(GetOdometerQueryHandler handler) : ControllerBase
{
    private readonly GetOdometerQueryHandler _handler = handler;

    /// <summary>
    /// Endpoint que retorna dados do hodômetro.
    /// </summary>
    /// <param name="query">Filtros de pesquisa (ex: datas, página, quantidade).</param>
    /// <returns>Dados paginados com resultados do hodômetro.</returns>
    /// <response code="200">Sucesso com dados retornados</response>
    /// <response code="400">Requisição inválida</response>
    /// <response code="500">Erro interno</response>
    [HttpGet]
    [ProducesResponseType(typeof(OdometerResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Get([FromQuery] GetOdometerQuery query)
    {
        var result = await _handler.HandleAsync(query);
        return result.ToActionResult();
    }
}
