namespace Day09.Core.StrategyPattern;

/// <summary>
/// class PricingService - 改進後的定價服務
/// </summary>
public class PricingService
{
    private readonly IDiscountStrategy _discountStrategy;
    private readonly ITaxStrategy _taxStrategy;

    /// <summary>
    /// 建構式
    /// </summary>
    /// <param name="discountStrategy">折扣策略</param>
    /// <param name="taxStrategy">稅收策略</param>
    public PricingService(IDiscountStrategy discountStrategy, ITaxStrategy taxStrategy)
    {
        this._discountStrategy = discountStrategy;
        this._taxStrategy = taxStrategy;
    }

    /// <summary>
    /// 計算價格
    /// </summary>
    /// <param name="product">商品</param>
    /// <param name="customer">客戶</param>
    /// <returns>最終價格</returns>
    public decimal CalculatePrice(Product product, Customer customer)
    {
        var basePrice = product.BasePrice;
        var discount = this._discountStrategy.Calculate(customer, product);
        var tax = this._taxStrategy.Calculate(product, customer.Location);

        return basePrice - discount + tax;
    }
}