namespace Day08.Core.Services;

/// <summary>
/// class PriceCalculationResult - 價格計算結果
/// </summary>
public class PriceCalculationResult
{
    /// <summary>
    /// 原始金額
    /// </summary>
    public decimal OriginalAmount { get; set; }

    /// <summary>
    /// VIP 折扣金額
    /// </summary>
    public decimal VipDiscount { get; set; }

    /// <summary>
    /// 優惠券折扣金額
    /// </summary>
    public decimal CouponDiscount { get; set; }

    /// <summary>
    /// 最終價格
    /// </summary>
    public decimal TotalPrice { get; set; }
}