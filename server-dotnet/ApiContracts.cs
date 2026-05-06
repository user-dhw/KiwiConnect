public sealed class ApiState
{
  public string Type { get; set; } = "SUCCESS";
  public string Msg { get; set; } = "Operation successful";
}

public sealed class ApiResponse
{
  public const string DefaultErrorType = "ERROE";
  public const string SuccessType = "SUCCESS";

  public ApiState State { get; set; } = new();
  public object Data { get; set; } = Array.Empty<object>();
  public int? Count { get; set; }
    = null;
  public int? Code { get; set; }
      = null;

  public static ApiResponse Success(object? data = null, int? count = null)
  {
    return new ApiResponse
    {
      State = new ApiState { Type = SuccessType, Msg = "Operation successful" },
      Data = data ?? Array.Empty<object>(),
      Count = count
    };
  }

  public static ApiResponse Error(string type = DefaultErrorType, string msg = "Operation failed", object? data = null, int? code = null)
  {
    return new ApiResponse
    {
      State = new ApiState { Type = type, Msg = msg },
      Data = data ?? Array.Empty<object>(),
      Code = code
    };
  }

  public static ApiResponse Unauthorized(string msg = "Authentication required")
  {
    return Error("UNAUTHORIZED", msg, code: 401);
  }

  public static ApiResponse Forbidden(string msg = "Access denied")
  {
    return Error("ERROR_NO_AUTHORITY", msg, code: 403);
  }

  public static ApiResponse NotFound(string msg = "Resource not found")
  {
    return Error("NOT_FOUND", msg, code: 404);
  }

  public static ApiResponse InvalidRequest(string msg = "Invalid parameters")
  {
    return Error("ERROR_PARAMS", msg, code: 400);
  }

  public static ApiResponse FromNullable(object? data, string notFoundMsg = "Resource not found")
  {
    return data is null ? NotFound(notFoundMsg) : Success(data);
  }
}
