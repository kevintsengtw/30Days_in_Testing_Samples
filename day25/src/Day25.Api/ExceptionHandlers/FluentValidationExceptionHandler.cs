using Microsoft.AspNetCore.Diagnostics;
using ValidationException = FluentValidation.ValidationException;

namespace Day25.Api.ExceptionHandlers;

/// <summary>
/// FluentValidation 異常處理器
/// </summary>
public class FluentValidationExceptionHandler : IExceptionHandler
{
    private readonly ILogger<FluentValidationExceptionHandler> _logger;

    public FluentValidationExceptionHandler(ILogger<FluentValidationExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        _logger.LogWarning("驗證失敗: {Errors}", string.Join(", ", validationException.Errors.Select(e => e.ErrorMessage)));

        var problemDetails = new ValidationProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest,
            Detail = "請檢查輸入資料是否正確",
            Instance = httpContext.Request.Path
        };

        foreach (var error in validationException.Errors)
        {
            if (problemDetails.Errors.ContainsKey(error.PropertyName))
            {
                problemDetails.Errors[error.PropertyName] = problemDetails.Errors[error.PropertyName]
                                                                          .Concat([error.ErrorMessage]).ToArray();
            }
            else
            {
                problemDetails.Errors.Add(error.PropertyName, [error.ErrorMessage]);
            }
        }

        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}