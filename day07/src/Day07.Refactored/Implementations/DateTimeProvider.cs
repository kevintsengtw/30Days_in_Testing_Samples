using Day07.Refactored.Abstractions;

namespace Day07.Refactored.Implementations;

/// <summary>
/// class DateTimeProvider - 日期時間提供者
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    /// <summary>
    /// 獲取現在的日期時間
    /// </summary>
    public DateTime Now => DateTime.Now;
}