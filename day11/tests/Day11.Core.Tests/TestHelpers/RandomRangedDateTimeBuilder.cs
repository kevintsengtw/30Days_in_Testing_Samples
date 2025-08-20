using System.Reflection;

namespace Day11.Core.Tests.TestHelpers;

/// <summary>
/// class RandomRangedDateTimeBuilder - 自訂的 DateTime 範圍建構器，可以指定特定屬性
/// </summary>
public class RandomRangedDateTimeBuilder : ISpecimenBuilder
{
    private readonly DateTime _minDate;
    private readonly DateTime _maxDate;
    private readonly HashSet<string> _targetProperties;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="minDate">最小日期</param>
    /// <param name="maxDate">最大日期</param>
    /// <param name="targetProperties">目標屬性名稱</param>
    public RandomRangedDateTimeBuilder(DateTime minDate, DateTime maxDate, params string[] targetProperties)
    {
        this._minDate = minDate;
        this._maxDate = maxDate;
        this._targetProperties = new HashSet<string>(targetProperties);
    }

    /// <summary>
    /// 建立物件
    /// </summary>
    /// <param name="request">請求</param>
    /// <param name="context">內容</param>
    /// <returns>建立的物件或 NoSpecimen</returns>
    public object Create(object request, ISpecimenContext context)
    {
        if (request is PropertyInfo propertyInfo &&
            propertyInfo.PropertyType == typeof(DateTime) &&
            this._targetProperties.Contains(propertyInfo.Name))
        {
            var range = this._maxDate - this._minDate;
            var randomTicks = (long)(Random.Shared.NextDouble() * range.Ticks);
            return this._minDate.AddTicks(randomTicks);
        }

        return new NoSpecimen();
    }
}