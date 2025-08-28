namespace Day19.WebApplication.Models;

/// <summary>
/// 貨運商輸出模型
/// </summary>
public class ShipperOutputModel
{
    /// <summary>
    /// 貨運商識別碼
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
}