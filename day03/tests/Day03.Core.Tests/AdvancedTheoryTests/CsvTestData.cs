namespace Day03.Core.Tests.AdvancedTheoryTests;

/// <summary>
/// class CsvTestData - 用於提供 CSV 測試資料的類別。
/// </summary>
public class CsvTestData : IEnumerable<object[]>
{
    public CsvTestData()
    {
        // 需要無參數建構式的情境：
        // xUnit 只要求 ClassData 類別有 public 無參數建構式，
        // 但如果你沒有自定義建構式，C# 會自動產生一個 public 無參數建構式。
        // 因此，只有在你自定義了其他建構式時，才必須明確寫出無參數建構式；否則可以省略，測試仍可正常執行。       
    }

    public IEnumerator<object[]> GetEnumerator()
    {
        var csvFilePath = Path.Combine("TestData", "calculations.csv");

        if (!File.Exists(csvFilePath))
        {
            // 如果檔案不存在，提供預設測試資料
            yield return [1, 2, 3];
            yield return [5, 5, 10];
            yield break;
        }

        var lines = File.ReadAllLines(csvFilePath);
        foreach (var line in lines.Skip(1)) // 跳過標題行
        {
            var values = line.Split(',');
            if (values.Length >= 3 &&
                int.TryParse(values[0], out var a) &&
                int.TryParse(values[1], out var b) &&
                int.TryParse(values[2], out var expected))
            {
                yield return [a, b, expected];
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}