using TUnit.Advanced.Core.Models;

namespace TUnit.Advanced.Core.Services;

/// <summary>
/// 運費計算服務實作
/// </summary>
public class ShippingCalculator : IShippingCalculator
{
    private const decimal FreeShippingThreshold = 1000m;
    private const decimal StandardShippingFee = 80m;
    private const decimal VipDiscountRate = 0.5m;

    /// <summary>
    /// 計算運費
    /// </summary>
    /// <param name="order">order</param>
    /// <returns></returns>
    public decimal CalculateShippingFee(Order order)
    {
        if (IsEligibleForFreeShipping(order))
        {
            return 0;
        }

        var baseFee = StandardShippingFee;

        // VIP 以上客戶享有運費折扣
        if (order.CustomerLevel >= CustomerLevel.VIP會員)
        {
            baseFee *= VipDiscountRate;
        }

        return baseFee;
    }

    /// <summary>
    /// 檢查是否符合免運費條件
    /// </summary>
    /// <param name="order">order</param>
    /// <returns></returns>
    public bool IsEligibleForFreeShipping(Order order)
    {
        // 訂單金額超過免運門檻
        if (order.SubTotal >= FreeShippingThreshold)
        {
            return true;
        }

        // 鑽石會員免運費
        if (order.CustomerLevel == CustomerLevel.鑽石會員)
        {
            return true;
        }

        return false;
    }
}