namespace Day16.Core;

/// <summary>
/// 訂單服務
/// </summary>
public class OrderService
{
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// 建立訂單服務實例
    /// </summary>
    /// <param name="timeProvider">時間提供者</param>
    /// <exception cref="ArgumentNullException">當 timeProvider 為 null 時拋出</exception>
    public OrderService(TimeProvider timeProvider)
    {
        this._timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    /// <summary>
    /// 檢查是否可以下單
    /// </summary>
    /// <returns>如果在營業時間內則回傳 true，否則回傳 false</returns>
    public bool CanPlaceOrder()
    {
        var now = this._timeProvider.GetLocalNow();
        var currentHour = now.Hour;

        // 營業時間：上午9點到下午5點
        return currentHour >= 9 && currentHour < 17;
    }

    /// <summary>
    /// 取得基於時間的折扣資訊
    /// </summary>
    /// <returns>折扣資訊字串</returns>
    public string GetTimeBasedDiscount()
    {
        var today = this._timeProvider.GetLocalNow().Date;

        if (today.DayOfWeek == DayOfWeek.Friday)
        {
            return "週五快樂：九折優惠";
        }

        if (today.Month == 12 && today.Day == 25)
        {
            return "聖誕特惠：八折優惠";
        }

        return "無優惠";
    }
}