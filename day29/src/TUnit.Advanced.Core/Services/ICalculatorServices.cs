using TUnit.Advanced.Core.Models;

namespace TUnit.Advanced.Core.Services;

/// <summary>
/// 折扣計算服務介面
/// </summary>
public interface IDiscountCalculator
{
    /// <summary>
    /// 計算訂單折扣
    /// </summary>
    Task<decimal> CalculateDiscountAsync(Order order, string discountCode);

    /// <summary>
    /// 驗證折扣碼是否有效
    /// </summary>
    Task<bool> ValidateDiscountCodeAsync(string discountCode, CustomerLevel customerLevel, decimal orderAmount);

    /// <summary>
    /// 取得折扣規則
    /// </summary>
    Task<DiscountRule?> GetDiscountRuleAsync(string discountCode);
}

/// <summary>
/// 運費計算服務介面
/// </summary>
public interface IShippingCalculator
{
    /// <summary>
    /// 計算運費
    /// </summary>
    decimal CalculateShippingFee(Order order);

    /// <summary>
    /// 檢查是否符合免運費條件
    /// </summary>
    bool IsEligibleForFreeShipping(Order order);
}