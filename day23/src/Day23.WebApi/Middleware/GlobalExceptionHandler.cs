using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace Day23.WebApi.Middleware;

/// <summary>
/// 全域異常處理器
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 嘗試處理異常
    /// </summary>
    /// <param name="httpContext">The httpContext</param>
    /// <param name="exception">The exception</param>
    /// <param name="cancellationToken">The cancellation</param>
    /// <returns></returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "發生未處理的異常: {Message}", exception.Message);

        var problemDetails = CreateProblemDetails(exception);

        httpContext.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await httpContext.Response.WriteAsync(json, cancellationToken);

        return true;
    }

    /// <summary>
    /// 根據異常類型建立對應的 ProblemDetails
    /// </summary>
    /// <param name="exception">The exception</param>
    /// <returns></returns>
    private static ProblemDetails CreateProblemDetails(Exception exception)
    {
        return exception switch
        {
            ArgumentException => new ProblemDetails
            {
                Type = "https://httpstatuses.com/400",
                Title = "參數錯誤",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = exception.Message
            },
            KeyNotFoundException => new ProblemDetails
            {
                Type = "https://httpstatuses.com/404",
                Title = "資源不存在",
                Status = (int)HttpStatusCode.NotFound,
                Detail = exception.Message
            },
            UnauthorizedAccessException => new ProblemDetails
            {
                Type = "https://httpstatuses.com/401",
                Title = "未授權",
                Status = (int)HttpStatusCode.Unauthorized,
                Detail = "您沒有權限執行此操作"
            },
            TimeoutException => new ProblemDetails
            {
                Type = "https://httpstatuses.com/408",
                Title = "請求超時",
                Status = (int)HttpStatusCode.RequestTimeout,
                Detail = "操作執行超時，請稍後再試"
            },
            InvalidOperationException => new ProblemDetails
            {
                Type = "https://httpstatuses.com/422",
                Title = "操作無效",
                Status = (int)HttpStatusCode.UnprocessableEntity,
                Detail = exception.Message
            },
            _ => new ProblemDetails
            {
                Type = "https://httpstatuses.com/500",
                Title = "內部伺服器錯誤",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = "發生未預期的錯誤，請聯絡系統管理員"
            }
        };
    }
}