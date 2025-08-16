namespace Day07.Refactored.Abstractions;

/// <summary>
/// interface IDateTimeProvider - 日期時間提供者介面
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// 獲取現在的日期時間
    /// </summary>
    DateTime Now { get; }
}