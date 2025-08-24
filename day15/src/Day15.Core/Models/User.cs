namespace Day15.Core.Models;

/// <summary>
/// 使用者實體
/// </summary>
public class User
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 名字
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// 姓氏
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 電話
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 生日
    /// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// 年齡
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// 家庭地址
    /// </summary>
    public Address? HomeAddress { get; set; }

    /// <summary>
    /// 公司
    /// </summary>
    public Company? Company { get; set; }

    /// <summary>
    /// 訂單列表
    /// </summary>
    public List<Order> Orders { get; set; } = new();
}