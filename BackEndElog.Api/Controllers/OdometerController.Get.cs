using BackEndElog.Api.Extensions;
using BackEndElog.Application.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BackEndElog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OdometerController(GetOdometerQueryHandler handler) : ControllerBase
{
    private readonly GetOdometerQueryHandler _handler = handler;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetOdometerQuery query)
    {
        var result = await _handler.HandleAsync(query);
        return result.ToActionResult();
    }
}
