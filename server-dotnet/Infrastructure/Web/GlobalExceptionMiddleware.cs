using System.Text.Json;

namespace InfoExchange.Api.Infrastructure.Web;

public sealed class GlobalExceptionMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<GlobalExceptionMiddleware> _logger;

  public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
      _logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);
      context.Response.StatusCode = StatusCodes.Status500InternalServerError;
      context.Response.ContentType = "application/json; charset=utf-8";

      var payload = ApiResponse.Error("INTERNAL_SERVER_ERROR", "Operation failed", code: 500);
      await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
  }
}
