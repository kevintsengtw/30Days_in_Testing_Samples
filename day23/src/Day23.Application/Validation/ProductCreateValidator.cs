using Day23.Application.Dtos;

namespace Day23.Application.Validation;

/// <summary>
/// 建立產品請求驗證器
/// </summary>
public class ProductCreateValidator : AbstractValidator<ProductCreateRequest>
{
    public ProductCreateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("產品名稱不能為空")
            .MaximumLength(100)
            .WithMessage("產品名稱不能超過 100 個字元");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("產品價格必須大於 0")
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("產品價格不能超過 999,999.99");
    }
}