using FluentValidation;

namespace Day25.Infrastructure.Validation;

/// <summary>
/// 產品建立請求驗證器
/// </summary>
public class ProductCreateRequestValidator : AbstractValidator<ProductCreateRequest>
{
    public ProductCreateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("產品名稱不能為空")
            .MaximumLength(100)
            .WithMessage("產品名稱不能超過 100 個字元");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("產品價格必須大於 0");
    }
}