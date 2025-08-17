namespace Day09.Core.StrategyPattern;

/// <summary>
/// class TaiwanTaxStrategy - 台灣稅收計算策略
/// </summary>
public class TaiwanTaxStrategy : ITaxStrategy
{
    /// <summary>
    /// 計算稅收
    /// </summary>
    /// <param name="product">商品</param>
    /// <param name="location">位置</param>
    /// <returns>稅收金額</returns>
    public decimal Calculate(Product product, Location location)
    {
        // 台灣稅收計算邏輯 - 現在是公開方法，容易測試
        return product.BasePrice * 0.05m;
    }
}