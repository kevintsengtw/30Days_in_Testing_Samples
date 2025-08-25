namespace Day16.Core;

/// <summary>
/// 交易服務
/// </summary>
public class TradingService
{
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// 建立交易服務實例
    /// </summary>
    /// <param name="timeProvider">時間提供者</param>
    public TradingService(TimeProvider timeProvider)
    {
        this._timeProvider = timeProvider;
    }

    /// <summary>
    /// 檢查是否在交易時間內
    /// </summary>
    /// <returns>如果在交易時間內則回傳 true</returns>
    public bool IsInTradingHours()
    {
        var now = this._timeProvider.GetLocalNow();
        var currentTime = now.TimeOfDay;

        // 交易時間：9:00-11:30, 13:00-15:00
        return (currentTime >= TimeSpan.FromHours(9) && currentTime <= TimeSpan.FromHours(11.5)) ||
               (currentTime >= TimeSpan.FromHours(13) && currentTime <= TimeSpan.FromHours(15));
    }

    /// <summary>
    /// 取得市場乘數
    /// </summary>
    /// <returns>市場乘數</returns>
    public decimal GetMarketMultiplier()
    {
        var now = this._timeProvider.GetLocalNow();

        return now.DayOfWeek switch
        {
            DayOfWeek.Saturday or DayOfWeek.Sunday => 0m, // 週末不交易
            DayOfWeek.Friday when now.Hour >= 14 => 1.1m, // 週五下午波動較大
            _ => 1.0m
        };
    }
}