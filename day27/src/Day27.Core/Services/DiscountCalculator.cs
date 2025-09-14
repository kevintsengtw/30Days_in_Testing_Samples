namespace Day27.Core.Services;

/// <summary>
/// 折扣計算器
/// </summary>
public class DiscountCalculator
{
    /// <summary>
    /// 計算折扣後的價格
    /// </summary>
    /// <param name="originalPrice">原始價格</param>
    /// <param name="discountRate">折扣率 (0.0 到 1.0 之間)</param>
    /// <returns>折扣後的價格</returns>
    /// <exception cref="ArgumentException">當輸入參數無效</exception>
    public decimal CalculateDiscountedPrice(decimal originalPrice, decimal discountRate)
    {
        if (originalPrice < 0)
        {
            throw new ArgumentException("原始價格不能為負數", nameof(originalPrice));
        }

        if (discountRate is < 0 or > 1)
        {
            throw new ArgumentException("折扣率必須在 0 到 1 之間", nameof(discountRate));
        }

        return originalPrice * (1 - discountRate);
    }

    /// <summary>
    /// 計算批量折扣
    /// </summary>
    /// <param name="originalPrice">原始價格</param>
    /// <param name="quantity">數量</param>
    /// <returns>批量折扣後的總價</returns>
    /// <exception cref="ArgumentException">當輸入參數無效</exception>
    public decimal CalculateBulkDiscount(decimal originalPrice, int quantity)
    {
        if (originalPrice < 0)
        {
            throw new ArgumentException("原始價格不能為負數", nameof(originalPrice));
        }

        if (quantity <= 0)
        {
            throw new ArgumentException("數量必須大於 0", nameof(quantity));
        }

        var totalPrice = originalPrice * quantity;
        var discountRate = GetBulkDiscountRate(quantity);

        return CalculateDiscountedPrice(totalPrice, discountRate);
    }

    /// <summary>
    /// 根據數量取得批量折扣率
    /// </summary>
    /// <param name="quantity">數量</param>
    /// <returns>折扣率</returns>
    private static decimal GetBulkDiscountRate(int quantity)
    {
        return quantity switch
        {
            >= 100 => 0.15m, // 15% 折扣
            >= 50 => 0.10m,  // 10% 折扣
            >= 10 => 0.05m,  // 5% 折扣
            _ => 0m          // 無折扣
        };
    }
}