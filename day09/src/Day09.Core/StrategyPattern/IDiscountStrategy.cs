namespace Day09.Core.StrategyPattern;

/// <summary>
/// interface IDiscountStrategy - 折扣計算策略介面
/// </summary>
public interface IDiscountStrategy
{
    /// <summary>
    /// 計算折扣
    /// </summary>
    /// <param name="customer">客戶</param>
    /// <param name="product">商品</param>
    /// <returns>折扣金額</returns>
    decimal Calculate(Customer customer, Product product);
}