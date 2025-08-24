namespace Day15.Core.Models;

/// <summary>
/// 訂單狀態列舉
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// 待處理
    /// </summary>
    Pending,

    /// <summary>
    /// 處理中
    /// </summary>
    Processing,

    /// <summary>
    /// 已出貨
    /// </summary>
    Shipped,

    /// <summary>
    /// 已送達
    /// </summary>
    Delivered,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled
}