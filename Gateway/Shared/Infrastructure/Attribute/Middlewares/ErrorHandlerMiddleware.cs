using System;
using System.Net;
using System.Text.Json;
namespace workstation_backend.Shared.Infrastructure.Attribute.Middlewares;

public class ErrorHandlerMiddleware
{
private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error]: {ex.Message}");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = JsonSerializer.Serialize(new
            {
                error = "Ocurrió un error inesperado.",
                detail = ex.Message
            });

            await context.Response.WriteAsync(result);
        }
    }
}
