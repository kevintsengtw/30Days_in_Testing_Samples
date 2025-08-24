namespace Day15.Core.Models;

/// <summary>
/// 公司實體
/// </summary>
public class Company
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 公司名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 行業
    /// </summary>
    public string Industry { get; set; } = string.Empty;

    /// <summary>
    /// 網站
    /// </summary>
    public string Website { get; set; } = string.Empty;

    /// <summary>
    /// 電話
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    public Address? Address { get; set; }

    /// <summary>
    /// 員工列表
    /// </summary>
    public List<User> Employees { get; set; } = new();
}