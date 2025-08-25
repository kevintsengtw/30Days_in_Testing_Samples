namespace Day16.Core;

/// <summary>
/// 排程系統的工作排程資訊
/// </summary>
public class JobSchedule
{
    /// <summary>
    /// 下次執行時間
    /// </summary>
    public DateTime NextExecutionTime { get; set; }

    /// <summary>
    /// Cron 表達式
    /// </summary>
    public string CronExpression { get; set; } = string.Empty;
}

/// <summary>
/// 排程服務
/// </summary>
public class ScheduleService
{
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// 建立排程服務實例
    /// </summary>
    /// <param name="timeProvider">時間提供者</param>
    public ScheduleService(TimeProvider timeProvider)
    {
        this._timeProvider = timeProvider;
    }

    /// <summary>
    /// 判斷是否應該執行工作
    /// </summary>
    /// <param name="schedule">工作排程</param>
    /// <returns>如果應該執行則回傳 true</returns>
    public bool ShouldExecuteJob(JobSchedule schedule)
    {
        var now = this._timeProvider.GetLocalNow();

        return schedule.NextExecutionTime <= now;
    }

    /// <summary>
    /// 計算下次執行時間
    /// </summary>
    /// <param name="schedule">工作排程</param>
    /// <returns>下次執行時間</returns>
    public DateTime CalculateNextExecution(JobSchedule schedule)
    {
        var now = this._timeProvider.GetLocalNow();

        return schedule.CronExpression switch
        {
            "0 0 * * *" => now.Date.AddDays(1),     // 每日午夜
            "0 0 * * 1" => this.GetNextMonday(now), // 每週一午夜
            _ => now.DateTime.AddHours(1)           // 預設每小時
        };
    }

    /// <summary>
    /// 取得下個週一的日期
    /// </summary>
    /// <param name="now">當前時間</param>
    /// <returns>下個週一的日期</returns>
    private DateTime GetNextMonday(DateTimeOffset now)
    {
        var daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
        return now.Date.AddDays(daysUntilMonday == 0 ? 7 : daysUntilMonday);
    }
}