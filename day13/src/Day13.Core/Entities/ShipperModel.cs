namespace Day13.Core.Entities;

/// <summary>
/// 出貨商實體
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
    /// 聯絡人
    /// </summary>
    public string ContactName { get; set; } = string.Empty;

    /// <summary>
    /// 電話號碼
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 傳真號碼
    /// </summary>
    public string Fax { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 城市
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// 郵遞區號
    /// </summary>
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// 國家
    /// </summary>
    public string Country { get; set; } = string.Empty;
}