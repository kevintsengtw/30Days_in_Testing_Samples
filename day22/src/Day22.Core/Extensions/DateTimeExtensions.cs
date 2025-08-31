namespace Day22.Core.Extensions;

/// <summary>
/// DateTime 擴充方法
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// 計算年齡
    /// </summary>
    /// <param name="birthDate">出生日期</param>
    /// <param name="timeProvider">時間提供者，預設使用系統時間</param>
    /// <returns>年齡</returns>
    public static int CalculateAge(this DateTime birthDate, TimeProvider? timeProvider = null)
    {
        var provider = timeProvider ?? TimeProvider.System;
        var today = provider.GetUtcNow().Date;
        var age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}