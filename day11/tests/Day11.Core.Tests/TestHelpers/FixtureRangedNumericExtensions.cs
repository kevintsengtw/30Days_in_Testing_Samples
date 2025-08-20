using System.Reflection;

namespace Day11.Core.Tests.TestHelpers;

/// <summary>
/// class FixtureRangedNumericExtensions - 提供數值範圍設定功能
/// </summary>
public static class FixtureRangedNumericExtensions
{
    /// <summary>
    /// 為指定型別的屬性添加數值範圍控制
    /// </summary>
    /// <typeparam name="T">包含屬性的類別型別</typeparam>
    /// <typeparam name="TValue">數值型別</typeparam>
    /// <param name="fixture">AutoFixture 實例</param>
    /// <param name="min">最小值</param>
    /// <param name="max">最大值</param>
    /// <param name="predicate">屬性判斷條件</param>
    /// <returns>AutoFixture 實例，支援流暢設定</returns>
    public static IFixture AddRandomRange<T, TValue>(this IFixture fixture,
                                                     TValue min, TValue max, Func<PropertyInfo, bool> predicate)
        where TValue : struct, IComparable, IConvertible
    {
        fixture.Customizations.Insert(0, new NumericRangeBuilder<TValue>(min, max, predicate));
        return fixture;
    }
}