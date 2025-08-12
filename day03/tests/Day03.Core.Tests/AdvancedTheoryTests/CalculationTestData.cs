namespace Day03.Core.Tests.AdvancedTheoryTests;

/// <summary>
/// class CalculationTestData - 提供計算測試的資料集。
/// </summary>
public class CalculationTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        // 基本運算測試
        yield return [10.0, 2.0, 5.0, "divide"];
        yield return [7.0, 2.0, 3.5, "divide"];
        yield return [-10.0, 2.0, -5.0, "divide"];

        // 乘法測試
        yield return [5.0, 3.0, 15.0, "multiply"];
        yield return [-2.0, 4.0, -8.0, "multiply"];
        yield return [0.0, 100.0, 0.0, "multiply"];

        // 邊界值測試
        yield return [double.MaxValue, 2.0, double.MaxValue / 2, "divide"];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}