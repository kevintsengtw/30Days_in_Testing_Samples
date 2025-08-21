namespace Day12.Core.Models;

/// <summary>
/// class CustomerJsonRecord - JSON 客戶資料記錄
/// </summary>
public class CustomerJsonRecord
{
    /// <summary>
    /// 客戶識別碼
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// 客戶姓名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件地址
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 客戶等級
    /// </summary>
    public string Level { get; set; } = string.Empty;

    /// <summary>
    /// 信用額度
    /// </summary>
    public decimal CreditLimit { get; set; }
}