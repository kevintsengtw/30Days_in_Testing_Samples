using Day10.Core.Models;

namespace Day10.Core.Services;

/// <summary>
/// 價格計算器
/// </summary>
public class PriceCalculator
{
    /// <summary>
    /// 計算總價
    /// </summary>
    public decimal Calculate(Product product, int quantity)
    {
        if (product == null || quantity <= 0)
        {
            return 0;
        }

        return product.Price * quantity;
    }
}