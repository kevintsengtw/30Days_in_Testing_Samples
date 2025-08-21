using Day12.Core.Enums;

namespace Day12.Core.Models;

/// <summary>
/// class Order - 訂單資料
/// </summary>
public class Order
{
    /// <summary>
    /// 訂單狀態
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// 訂單編號
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// 訂單金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 檢查是否可轉換到指定狀態
    /// </summary>
    /// <param name="newStatus">目標狀態</param>
    /// <returns>是否可轉換</returns>
    public bool CanTransitionTo(OrderStatus newStatus)
    {
        return (this.Status, newStatus) switch
        {
            (OrderStatus.Created, OrderStatus.Confirmed) => true,
            (OrderStatus.Confirmed, OrderStatus.Shipped) => true,
            (OrderStatus.Shipped, OrderStatus.Delivered) => true,
            (OrderStatus.Delivered, OrderStatus.Completed) => true,
            _ => false
        };
    }
}