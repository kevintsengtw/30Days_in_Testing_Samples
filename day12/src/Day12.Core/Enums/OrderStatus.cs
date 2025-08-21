namespace Day12.Core.Enums;

/// <summary>
/// enum OrderStatus - 訂單狀態列舉
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// 已建立
    /// </summary>
    Created,

    /// <summary>
    /// 已確認
    /// </summary>
    Confirmed,

    /// <summary>
    /// 已出貨
    /// </summary>
    Shipped,

    /// <summary>
    /// 已送達
    /// </summary>
    Delivered,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled
}