namespace Day15.Core.Models;

/// <summary>
/// 地址實體
/// </summary>
public class Address
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 街道
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
}