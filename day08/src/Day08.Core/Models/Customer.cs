namespace Day08.Core.Models;

/// <summary>
/// class Customer - 客戶資訊
/// </summary>
public class Customer
{
    /// <summary>
    /// 客戶編號
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 客戶類型
    /// </summary>
    public CustomerType Type { get; set; }

    /// <summary>
    /// 購買歷史總金額
    /// </summary>
    public decimal PurchaseHistory { get; set; }
}