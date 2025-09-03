using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using ValidationException = FluentValidation.ValidationException;

namespace Day25.Api.Configuration;

/// <summary>
/// 驗證過濾器
/// </summary>
public interface IValidationFilter
{
}

/// <summary>
/// 模型驗證過濾器
/// </summary>
public class ValidationFilter : IAsyncActionFilter, IValidationFilter
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var parameter in context.ActionDescriptor.Parameters)
        {
            var parameterValue = context.ActionArguments.ContainsKey(parameter.Name)
                ? context.ActionArguments[parameter.Name]
                : null;

            if (parameterValue != null)
            {
                var validatorType = typeof(IValidator<>).MakeGenericType(parameter.ParameterType);
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationContext = new ValidationContext<object>(parameterValue);
                    var validationResult = await validator.ValidateAsync(validationContext);

                    if (!validationResult.IsValid)
                    {
                        throw new ValidationException(validationResult.Errors);
                    }
                }
            }
        }

        await next();
    }
}