namespace Day13.Core.Models;

/// <summary>
/// 出貨商模型
/// </summary>
public class ShipperModel
{
    /// <summary>
    /// 出貨商編號
    /// </summary>
    public int ShipperId { get; set; }

    /// <summary>
    /// 公司名稱
    /// </summary>
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// 聯絡電話
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 建立日期
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// 更新日期
    /// </summary>
    public DateTime? UpdatedDate { get; set; }
}