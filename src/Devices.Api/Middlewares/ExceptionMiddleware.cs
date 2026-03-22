using System.Net;
using System.Text.Json;
using Devices.Application.Common;

namespace Devices.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = Result<object>.Failure(
                message: "Internal server error",
                statusCode: HttpStatusCode.InternalServerError);

            var json = JsonSerializer.Serialize(result);

            await context.Response.WriteAsync(json);
        }
    }
}