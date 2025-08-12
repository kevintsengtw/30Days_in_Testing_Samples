namespace Day03.Core.Tests.TestDataProviders;

/// <summary>
/// interface ITestDataProvider - 提供測試資料的介面。
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ITestDataProvider<T>
{
    /// <summary>
    /// 提供有效的測試資料。
    /// </summary>
    /// <returns></returns>
    IEnumerable<T> GetValidData();

    /// <summary>
    /// 提供無效的測試資料。
    /// </summary>
    /// <returns></returns>
    IEnumerable<T> GetInvalidData();

    /// <summary>
    /// 提供邊界測試資料。
    /// </summary>
    /// <returns></returns>
    IEnumerable<T> GetBoundaryData();

    /// <summary>
    /// 提供範例測試資料。
    /// </summary>
    /// <returns></returns>
    T GetSampleData();
}