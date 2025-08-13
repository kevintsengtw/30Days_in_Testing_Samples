namespace Day04.Domain.Models;

/// <summary>
/// class Address - 用於表示地址的模型。
/// </summary>
public class Address
{
    /// <summary>
    /// 地址街道
    /// </summary>
    /// <value></value>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// 城市
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// 州/省
    /// </summary>
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// 郵遞區號
    /// </summary>
    public string ZipCode { get; set; } = string.Empty;
}