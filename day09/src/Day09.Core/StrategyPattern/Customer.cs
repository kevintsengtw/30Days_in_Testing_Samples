namespace Day09.Core.StrategyPattern;

/// <summary>
/// class Customer - 客戶
/// </summary>
public class Customer
{
    /// <summary>
    /// 是否為 VIP 客戶
    /// </summary>
    public bool IsVip { get; set; }

    /// <summary>
    /// 客戶位置
    /// </summary>
    public Location Location { get; set; } = new();
}