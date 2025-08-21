namespace Day12.Core.Models;

/// <summary>
/// class Customer - 客戶資料
/// </summary>
public class Customer
{
    /// <summary>
    /// 客戶個人資料
    /// </summary>
    public Person Person { get; set; } = new();

    /// <summary>
    /// 客戶類型（VIP、一般客戶等）
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// 信用額度
    /// </summary>
    public decimal CreditLimit { get; set; }

    /// <summary>
    /// 檢查是否可下訂單
    /// </summary>
    /// <param name="orderAmount">訂單金額</param>
    /// <returns>是否可下訂單</returns>
    public bool CanPlaceOrder(decimal orderAmount)
    {
        return orderAmount <= this.CreditLimit;
    }
}