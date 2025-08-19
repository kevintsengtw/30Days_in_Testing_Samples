using Day10.Core.Enums;

namespace Day10.Core.Models;

/// <summary>
/// 客戶實體
/// </summary>
public class Customer
{
    /// <summary>
    /// 客戶編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 客戶姓名
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 住址
    /// </summary>
    public Address? Address { get; set; }

    /// <summary>
    /// 聯絡資訊
    /// </summary>
    public ContactInfo? ContactInfo { get; set; }

    /// <summary>
    /// 加入日期
    /// </summary>
    public DateTime JoinDate { get; set; }

    /// <summary>
    /// 客戶類型
    /// </summary>
    public CustomerType Type { get; set; }

    /// <summary>
    /// 年齡
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// 總消費金額
    /// </summary>
    public decimal TotalSpent { get; set; }

    /// <summary>
    /// 訂單清單 - 注意：為避免循環參考，在 AutoFixture 測試中需要特別處理
    /// </summary>
    public List<Order> Orders { get; set; } = new();

    /// <summary>
    /// 取得客戶等級
    /// </summary>
    public CustomerLevel GetLevel()
    {
        return this.TotalSpent switch
        {
            >= 100000 => CustomerLevel.Diamond,
            >= 50000 => CustomerLevel.Gold,
            >= 10000 => CustomerLevel.Silver,
            _ => CustomerLevel.Bronze
        };
    }

    /// <summary>
    /// 是否可獲得折扣
    /// </summary>
    public bool CanGetDiscount()
    {
        return this.TotalSpent >= 1000 && this.Orders.Count >= 5;
    }
}