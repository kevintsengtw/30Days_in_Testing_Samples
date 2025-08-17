namespace Day09.Core.StrategyPattern;

/// <summary>
/// interface ITaxStrategy - 稅收計算策略介面
/// </summary>
public interface ITaxStrategy
{
    /// <summary>
    /// 計算稅收
    /// </summary>
    /// <param name="product">商品</param>
    /// <param name="location">位置</param>
    /// <returns>稅收金額</returns>
    decimal Calculate(Product product, Location location);
}