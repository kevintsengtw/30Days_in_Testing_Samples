namespace Day15.TestLibrary.TestData;

/// <summary>
/// 統一的測試資料產生介面
/// </summary>
public interface ITestDataGenerator
{
    /// <summary>
    /// 產生單一物件
    /// </summary>
    T Generate<T>();

    /// <summary>
    /// 產生指定數量的物件
    /// </summary>
    IEnumerable<T> Generate<T>(int count);

    /// <summary>
    /// 產生物件並允許後續設定
    /// </summary>
    T Generate<T>(Action<T> configure);

    /// <summary>
    /// 產生物件並允許建構參數客製化
    /// </summary>
    T Generate<T>(params object[] constructorParameters);
}