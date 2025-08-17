using Day08.Core.Models;

namespace Day08.Core.Services;

/// <summary>
/// class ProductService - 商品服務
/// </summary>
public class ProductService
{
    /// <summary>
    /// 計算折扣
    /// </summary>
    /// <param name="customer">客戶資訊</param>
    /// <param name="product">商品資訊</param>
    /// <returns>折扣百分比</returns>
    public decimal CalculateDiscount(Customer customer, Product product)
    {
        decimal discount = 0;

        // VIP 客戶享有基本折扣
        if (customer.Type == CustomerType.VIP)
        {
            discount += 10;
        }

        // 高價商品額外折扣
        if (product.Price >= 1000)
        {
            discount += 5;
        }

        // 購買歷史折扣
        if (customer.PurchaseHistory >= 10000)
        {
            discount += 5;
        }

        return Math.Min(discount, 20); // 最大折扣 20%
    }

    /// <summary>
    /// 計算總價格（包含複雜折扣邏輯）
    /// </summary>
    /// <param name="customer">客戶資訊</param>
    /// <param name="items">訂單項目</param>
    /// <param name="couponCode">優惠券代碼</param>
    /// <returns>計算結果</returns>
    public PriceCalculationResult CalculateTotalPrice(Customer customer, OrderItem[] items, string? couponCode = null)
    {
        var originalAmount = items.Sum(item => item.Price * item.Quantity);

        var result = new PriceCalculationResult
        {
            OriginalAmount = originalAmount
        };

        // VIP 折扣 15%
        if (customer.Type == CustomerType.VIP)
        {
            result.VipDiscount = originalAmount * 0.15m;
        }

        // 優惠券折扣
        if (!string.IsNullOrEmpty(couponCode) && couponCode == "SUMMER2024")
        {
            result.CouponDiscount = originalAmount * 0.1m;
        }

        result.TotalPrice = originalAmount - result.VipDiscount - result.CouponDiscount;
        return result;
    }
}