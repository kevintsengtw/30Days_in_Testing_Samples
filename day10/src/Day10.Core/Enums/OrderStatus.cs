namespace Day10.Core.Enums;

/// <summary>
/// 訂單狀態
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
    /// 已完成
    /// </summary>
    Completed,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled
}