namespace Day10.Core.Models;

/// <summary>
/// 住址資訊
/// </summary>
public class Address
{
    /// <summary>
    /// 街道地址
    /// </summary>
    public string Street { get; set; } = string.Empty;

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

    /// <summary>
    /// 地理位置
    /// </summary>
    public GeoLocation? Location { get; set; }
}