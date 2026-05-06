using Microsoft.AspNetCore.Mvc;

namespace InfoExchange.Api.Infrastructure.Web;

public static class ApiResponseHttpExtensions
{
  public static IActionResult FromApiResponse(this ControllerBase controller, ApiResponse response)
  {
    ArgumentNullException.ThrowIfNull(controller);

    return controller.StatusCode(GetStatusCode(response), response);
  }

  public static int GetStatusCode(ApiResponse response)
  {
    ArgumentNullException.ThrowIfNull(response);

    if (string.Equals(response.State?.Type, "SUCCESS", StringComparison.OrdinalIgnoreCase))
    {
      return StatusCodes.Status200OK;
    }

    if (response.Code is >= 100 and <= 599)
    {
      return response.Code.Value;
    }

    var type = Normalize(response.State?.Type);

    if (type.Contains("NO_AUTHORITY", StringComparison.Ordinal) || type.Contains("FORBIDDEN", StringComparison.Ordinal))
    {
      return StatusCodes.Status403Forbidden;
    }

    if (type.Contains("UNAUTHORIZED", StringComparison.Ordinal) || type.Contains("INVALID_CREDENTIALS", StringComparison.Ordinal))
    {
      return StatusCodes.Status401Unauthorized;
    }

    if (type.Contains("NOT_FOUND", StringComparison.Ordinal) || type.Contains("NO_RECORDS_UPDATED", StringComparison.Ordinal))
    {
      return StatusCodes.Status404NotFound;
    }

    if (type.Contains("EXIST", StringComparison.Ordinal) || type.Contains("CONFLICT", StringComparison.Ordinal))
    {
      return StatusCodes.Status409Conflict;
    }

    if (type.Contains("MISSING_", StringComparison.Ordinal) || type.Contains("INVALID_", StringComparison.Ordinal) || type.Contains("ERROR_PARAMS", StringComparison.Ordinal) || type.Contains("PARAM", StringComparison.Ordinal))
    {
      return StatusCodes.Status400BadRequest;
    }

    if (type.EndsWith("_FAILED", StringComparison.Ordinal))
    {
      return StatusCodes.Status500InternalServerError;
    }

    return StatusCodes.Status400BadRequest;
  }

  private static string Normalize(string? value)
  {
    return (value ?? string.Empty).Trim().ToUpperInvariant().Replace(" ", string.Empty);
  }
}