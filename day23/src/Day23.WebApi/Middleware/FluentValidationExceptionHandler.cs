using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;

namespace Day23.WebApi.Middleware;

/// <summary>
/// FluentValidation 專用異常處理器
/// </summary>
public class FluentValidationExceptionHandler : IExceptionHandler
{
    private readonly ILogger<FluentValidationExceptionHandler> _logger;

    public FluentValidationExceptionHandler(ILogger<FluentValidationExceptionHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 嘗試處理異常
    /// </summary>
    /// <param name="httpContext">The httpContext</param>
    /// <param name="exception">The exception</param>
    /// <param name="cancellationToken">The cancellationToken</param>
    /// <returns></returns>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        _logger.LogWarning(validationException, "驗證失敗: {Message}", validationException.Message);

        var problemDetails = new ValidationProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            Title = "One or more validation errors occurred.",
            Status = (int)HttpStatusCode.BadRequest,
            Detail = "輸入的資料包含驗證錯誤，請檢查後重新提交。",
            Instance = httpContext.Request.Path
        };

        foreach (var error in validationException.Errors)
        {
            var propertyName = error.PropertyName;
            var errorMessage = error.ErrorMessage;

            if (problemDetails.Errors.TryGetValue(propertyName, out var value))
            {
                var existingErrors = value.ToList();
                existingErrors.Add(errorMessage);
                problemDetails.Errors[propertyName] = existingErrors.ToArray();
            }
            else
            {
                problemDetails.Errors.Add(propertyName, [errorMessage]);
            }
        }

        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        httpContext.Response.ContentType = "application/problem+json";

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await httpContext.Response.WriteAsync(json, cancellationToken);

        return true;
    }
}