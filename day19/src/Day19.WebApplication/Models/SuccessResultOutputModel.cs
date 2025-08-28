namespace Day19.WebApplication.Models;

/// <summary>
/// 成功結果輸出模型
/// </summary>
/// <typeparam name="T">資料類型</typeparam>
public class SuccessResultOutputModel<T>
{
    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 資料
    /// </summary>
    public T Data { get; set; } = default!;
}