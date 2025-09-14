namespace Day27.Core.Models;

/// <summary>
/// 訂單狀態列舉
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// 待處理
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 已確認
    /// </summary>
    Confirmed = 2,

    /// <summary>
    /// 已付款
    /// </summary>
    Paid = 3,

    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled = 4
}