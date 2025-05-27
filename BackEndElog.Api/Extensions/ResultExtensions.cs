using BackEndElog.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace BackEndElog.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(result.Value);

        var statusCode = result.Error?.Code switch
        {
            >= 400 and < 600 => result.Error.Code,
            _ => 400
        };

        return new ObjectResult(new { error = result.Error?.Description })
        {
            StatusCode = statusCode
        };
    }
}