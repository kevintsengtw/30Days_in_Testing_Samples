using System.Reflection;

namespace Day11.Core.Tests.TestHelpers;

/// <summary>
/// class RandomRangedNumericSequenceBuilder
/// </summary>
/// <remarks>
/// 第一個版本的數值範圍建構器（簡單的屬性名稱比對）
/// 這個版本可能會因為 AutoFixture 內建建構器的優先順序而失效
/// </remarks>
public class RandomRangedNumericSequenceBuilder : ISpecimenBuilder
{
    private readonly int _min;
    private readonly int _max;
    private readonly HashSet<string> _targetProperties;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="targetProperties">目標屬性名稱</param>
    public RandomRangedNumericSequenceBuilder(int min, int max, params string[] targetProperties)
    {
        this._min = min;
        this._max = max;
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
            propertyInfo.PropertyType == typeof(int) &&
            this._targetProperties.Contains(propertyInfo.Name))
        {
            return Random.Shared.Next(this._min, this._max);
        }

        return new NoSpecimen();
    }
}