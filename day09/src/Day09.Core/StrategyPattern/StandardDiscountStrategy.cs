namespace Day09.Core.StrategyPattern;

/// <summary>
/// class StandardDiscountStrategy - 標準折扣計算策略
/// </summary>
public class StandardDiscountStrategy : IDiscountStrategy
{
    /// <summary>
    /// 計算折扣
    /// </summary>
    /// <param name="customer">客戶</param>
    /// <param name="product">商品</param>
    /// <returns>折扣金額</returns>
    public decimal Calculate(Customer customer, Product product)
    {
        // 標準折扣邏輯 - 現在是公開方法，容易測試
        if (customer.IsVip)
        {
            return product.BasePrice * 0.1m;
        }

        return 0m;
    }
}