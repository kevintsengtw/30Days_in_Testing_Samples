using System.Reflection;

namespace Day11.Core.Tests.TestHelpers;

/// <summary>
/// class NumericRangeBuilder - 泛型數值範圍建構器，支援所有數值型別的範圍控制
/// </summary>
/// <typeparam name="TValue">數值型別，必須是結構且可比較和轉換</typeparam>
public class NumericRangeBuilder<TValue> : ISpecimenBuilder
    where TValue : struct, IComparable, IConvertible
{
    private readonly TValue _min;
    private readonly TValue _max;
    private readonly Func<PropertyInfo, bool> _predicate;

    /// <summary>
    /// 建立泛型數值範圍建構器
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="predicate">屬性判斷條件</param>
    public NumericRangeBuilder(TValue min, TValue max, Func<PropertyInfo, bool> predicate)
    {
        this._min = min;
        this._max = max;
        this._predicate = predicate;
    }

    /// <summary>
    /// 建立數值範圍內的隨機值
    /// </summary>
    /// <param name="request">請求物件</param>
    /// <param name="context">AutoFixture 上下文</param>
    /// <returns>隨機數值或 NoSpecimen</returns>
    public object Create(object request, ISpecimenContext context)
    {
        if (request is PropertyInfo propertyInfo &&
            propertyInfo.PropertyType == typeof(TValue) &&
            this._predicate(propertyInfo))
        {
            return this.GenerateRandomValue();
        }

        return new NoSpecimen();
    }

    /// <summary>
    /// 產生指定範圍內的隨機數值
    /// </summary>
    /// <returns>隨機數值</returns>
    private TValue GenerateRandomValue()
    {
        // 統一轉換為 decimal 進行計算
        var minDecimal = Convert.ToDecimal(this._min);
        var maxDecimal = Convert.ToDecimal(this._max);
        var range = maxDecimal - minDecimal;
        var randomValue = minDecimal + (decimal)Random.Shared.NextDouble() * range;

        // 根據目標型別轉換回去
        return typeof(TValue).Name switch
        {
            nameof(Int32) => (TValue)(object)(int)randomValue,
            nameof(Int64) => (TValue)(object)(long)randomValue,
            nameof(Int16) => (TValue)(object)(short)randomValue,
            nameof(Byte) => (TValue)(object)(byte)randomValue,
            nameof(Single) => (TValue)(object)(float)randomValue,
            nameof(Double) => (TValue)(object)(double)randomValue,
            nameof(Decimal) => (TValue)(object)randomValue,
            _ => throw new NotSupportedException($"Type {typeof(TValue).Name} is not supported")
        };
    }
}