namespace Day19.WebApplication.Data.Entities;

/// <summary>
/// 貨運商實體
/// </summary>
public class Shipper
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