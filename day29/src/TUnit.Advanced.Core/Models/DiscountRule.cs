namespace TUnit.Advanced.Core.Models;

/// <summary>
/// 折扣規則實體類別
/// </summary>
public class DiscountRule
{
    /// <summary>
    /// 折扣碼
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 折扣類型
    /// </summary>
    public DiscountType Type { get; set; }

    /// <summary>
    /// 折扣值（百分比為 0-100，金額為實際數值）
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// 最低消費門檻
    /// </summary>
    public decimal MinimumAmount { get; set; }

    /// <summary>
    /// 最大折扣金額
    /// </summary>
    public decimal? MaxDiscountAmount { get; set; }

    /// <summary>
    /// 適用的客戶等級
    /// </summary>
    public List<CustomerLevel> ApplicableCustomerLevels { get; set; } = [];

    /// <summary>
    /// 是否有效
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime EndDate { get; set; }
}