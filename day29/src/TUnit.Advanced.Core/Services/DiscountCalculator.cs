using TUnit.Advanced.Core.Models;

namespace TUnit.Advanced.Core.Services;

/// <summary>
/// 折扣計算服務實作
/// </summary>
public class DiscountCalculator : IDiscountCalculator
{
    private readonly IDiscountRepository _discountRepository;
    private readonly ILogger<DiscountCalculator> _logger;

    public DiscountCalculator(IDiscountRepository discountRepository, ILogger<DiscountCalculator> logger)
    {
        _discountRepository = discountRepository;
        _logger = logger;
    }

    /// <summary>
    /// 計算訂單折扣
    /// </summary>
    /// <param name="order">order</param>
    /// <param name="discountCode">discountCode</param>
    /// <returns></returns>
    public async Task<decimal> CalculateDiscountAsync(Order order, string discountCode)
    {
        if (string.IsNullOrWhiteSpace(discountCode))
        {
            return 0;
        }

        var discountRule = await _discountRepository.GetDiscountRuleByCodeAsync(discountCode);
        if (discountRule == null || !IsDiscountRuleValid(discountRule, order))
        {
            _logger.LogWarning("折扣碼 {DiscountCode} 無效或不適用", discountCode);
            return 0;
        }

        var discount = discountRule.Type switch
        {
            DiscountType.百分比折扣 => order.SubTotal * (discountRule.Value / 100),
            DiscountType.固定金額折扣 => discountRule.Value,
            DiscountType.滿額折扣 => order.SubTotal >= discountRule.MinimumAmount ? discountRule.Value : 0,
            _ => 0
        };

        // 套用最大折扣限制
        if (discountRule.MaxDiscountAmount.HasValue)
        {
            discount = Math.Min(discount, discountRule.MaxDiscountAmount.Value);
        }

        // 折扣不能超過訂單金額
        discount = Math.Min(discount, order.SubTotal);

        _logger.LogInformation("訂單 {OrderId} 套用折扣碼 {DiscountCode}，折扣金額: {Discount}",
                               order.OrderId, discountCode, discount);

        return discount;
    }

    /// <summary>
    /// 驗證折扣碼是否有效
    /// </summary>
    /// <param name="discountCode">discountCode</param>
    /// <param name="customerLevel">customerLevel</param>
    /// <param name="orderAmount">orderAmount</param>
    /// <returns></returns>
    public async Task<bool> ValidateDiscountCodeAsync(string discountCode, CustomerLevel customerLevel, decimal orderAmount)
    {
        if (string.IsNullOrWhiteSpace(discountCode))
        {
            return false;
        }

        var discountRule = await _discountRepository.GetDiscountRuleByCodeAsync(discountCode);
        if (discountRule == null)
        {
            return false;
        }

        var now = DateTime.UtcNow;
        return discountRule.IsActive
               && now >= discountRule.StartDate
               && now <= discountRule.EndDate
               && orderAmount >= discountRule.MinimumAmount
               && (discountRule.ApplicableCustomerLevels.Count == 0 ||
                   discountRule.ApplicableCustomerLevels.Contains(customerLevel));
    }

    /// <summary>
    /// 取得折扣規則
    /// </summary>
    /// <param name="discountCode">discountCode</param>
    /// <returns></returns>
    public async Task<DiscountRule?> GetDiscountRuleAsync(string discountCode)
    {
        return await _discountRepository.GetDiscountRuleByCodeAsync(discountCode);
    }

    /// <summary>
    /// 驗證折扣規則是否適用於訂單
    /// </summary>
    /// <param name="rule">rule</param>
    /// <param name="order">order</param>
    /// <returns></returns>
    private static bool IsDiscountRuleValid(DiscountRule rule, Order order)
    {
        var now = DateTime.UtcNow;
        return rule.IsActive
               && now >= rule.StartDate
               && now <= rule.EndDate
               && order.SubTotal >= rule.MinimumAmount
               && (rule.ApplicableCustomerLevels.Count == 0 ||
                   rule.ApplicableCustomerLevels.Contains(order.CustomerLevel));
    }
}