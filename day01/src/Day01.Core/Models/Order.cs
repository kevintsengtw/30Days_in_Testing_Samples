using Day01.Core.Enums;

namespace Day01.Core.Models;

/// <summary>
/// class Order - 訂單模型類別
/// </summary>
public class Order
{
    public string Prefix { get; set; } = string.Empty;
    
    public string Number { get; set; } = string.Empty;
    
    public decimal Amount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Created;

    /// <summary>
    /// 處理訂單，將狀態更新為已處理
    /// </summary>
    public void Process()
    {
        this.Status = OrderStatus.Processed;
    }
}