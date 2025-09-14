namespace Day27.Core.Models;

/// <summary>
/// 使用者狀態列舉
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// 啟用狀態
    /// </summary>
    Active = 1,

    /// <summary>
    /// 非啟用狀態
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// 暫停狀態
    /// </summary>
    Suspended = 3
}