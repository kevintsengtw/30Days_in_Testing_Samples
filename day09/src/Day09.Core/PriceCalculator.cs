namespace Day09.Core;

/// <summary>
/// class PriceCalculator - 價格計算器（僅供內部使用）
/// </summary>
internal class PriceCalculator
{
    /// <summary>
    /// 計算商品等級
    /// </summary>
    /// <param name="price">商品價格</param>
    /// <returns>商品等級</returns>
    internal string CalculatePriceLevel(decimal price)
    {
        return price switch
        {
            >= 10000 => "豪華級",
            >= 5000 => "高級",
            >= 1000 => "中級",
            > 0 => "經濟級",
            _ => "無效價格"
        };
    }

    /// <summary>
    /// 計算折扣後價格
    /// </summary>
    /// <param name="originalPrice">原價</param>
    /// <param name="discountRate">折扣率 (0-1之間)</param>
    /// <returns>折扣後價格</returns>
    internal decimal CalculateDiscountedPrice(decimal originalPrice, decimal discountRate)
    {
        if (discountRate is < 0 or > 1)
        {
            throw new ArgumentException("折扣率必須在0到1之間");
        }

        return originalPrice * (1 - discountRate);
    }
}