namespace Day10.Core.Models;

/// <summary>
/// 聯絡資訊
/// </summary>
public class ContactInfo
{
    /// <summary>
    /// 電話號碼
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 手機號碼
    /// </summary>
    public string MobilePhone { get; set; } = string.Empty;

    /// <summary>
    /// 傳真號碼
    /// </summary>
    public string Fax { get; set; } = string.Empty;
}