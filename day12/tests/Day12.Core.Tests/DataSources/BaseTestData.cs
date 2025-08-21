namespace Day12.Core.Tests.DataSources;

/// <summary>
/// 測試資料來源基底類別
/// </summary>
public abstract class BaseTestData
{
    protected static string GetTestDataPath(string fileName)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "TestData", fileName);
    }
}