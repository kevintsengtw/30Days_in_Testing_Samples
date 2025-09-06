namespace TUnit.Demo.Core;

/// <summary>
/// 時間服務類別，展示時間相關的測試案例
/// </summary>
public class TimeService
{
    private readonly TimeProvider _timeProvider;

    public TimeService(TimeProvider? timeProvider = null)
    {
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <summary>
    /// 取得當前時間
    /// </summary>
    public DateTime GetCurrentTime()
    {
        return _timeProvider.GetLocalNow().DateTime;
    }

    /// <summary>
    /// 建立使用者時設定時間戳記
    /// </summary>
    public User CreateUser(string email)
    {
        return new User
        {
            Email = email,
            CreatedAt = _timeProvider.GetLocalNow().DateTime,
            Id = Guid.NewGuid()
        };
    }

    /// <summary>
    /// 計算經過的時間
    /// </summary>
    public TimeSpan CalculateElapsed(DateTime startTime)
    {
        return _timeProvider.GetLocalNow().DateTime - startTime;
    }

    /// <summary>
    /// 檢查是否在營業時間內（9:00-17:00）
    /// </summary>
    public bool IsBusinessHours()
    {
        var currentHour = _timeProvider.GetLocalNow().Hour;
        return currentHour is >= 9 and < 17;
    }

    /// <summary>
    /// 取得基於時間的折扣
    /// </summary>
    public string GetTimeBasedDiscount()
    {
        var today = _timeProvider.GetLocalNow().Date;

        if (today.DayOfWeek == DayOfWeek.Friday)
        {
            return "週五快樂：九折優惠";
        }

        if (today is { Month: 12, Day: 25 })
        {
            return "聖誕特惠：八折優惠";
        }

        return "無優惠";
    }
}

/// <summary>
/// 使用者類別
/// </summary>
public class User
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}