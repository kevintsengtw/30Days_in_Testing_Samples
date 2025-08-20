using System.Reflection;

namespace Day11.Core.Tests.TestHelpers;

/// <summary>
/// class ImprovedRandomRangedNumericSequenceBuilder - 改進版本的數值範圍建構器，使用 Func predicate 進行精確控制
/// </summary>
public class ImprovedRandomRangedNumericSequenceBuilder : ISpecimenBuilder
{
    private readonly int _min;
    private readonly int _max;
    private readonly Func<PropertyInfo, bool> _predicate;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="predicate">屬性判斷條件</param>
    public ImprovedRandomRangedNumericSequenceBuilder(int min, int max, Func<PropertyInfo, bool> predicate)
    {
        this._min = min;
        this._max = max;
        this._predicate = predicate;
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
            propertyInfo.PropertyType == typeof(int) &&
            this._predicate(propertyInfo))
        {
            return Random.Shared.Next(this._min, this._max);
        }

        return new NoSpecimen();
    }
}