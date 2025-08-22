using System.ComponentModel.DataAnnotations;

namespace Day13.Core.Validation;

/// <summary>
/// Class ModelValidator.
/// </summary>
public static class ModelValidator
{
    /// <summary>
    /// Validates the specified model.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="model">The model.</param>
    /// <param name="parameterName">parameterName</param>
    /// <exception cref="ArgumentException"></exception>
    public static void Validate<T>(T model, string parameterName) where T : class
    {
        if (model is null)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                parameterName = nameof(model);
            }

            throw new ArgumentNullException(paramName: parameterName, message: $"The value '{parameterName}' cannot be null.");
        }

        var errors = new List<ValidationResult>();
        var validateResult = Validator.TryValidateObject(model, new ValidationContext(model), errors, true);
        if (validateResult || errors.Count.Equals(0))
        {
            return;
        }

        var error = errors.FirstOrDefault();
        throw new ArgumentException(message: $"{error?.ErrorMessage}", paramName: error?.MemberNames.FirstOrDefault());
    }
}