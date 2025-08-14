namespace Day05.Domain.Services.ProcessingServices;

/// <summary>
/// class ParallelService - 並行服務
/// </summary>
public class ParallelService
{
    /// <summary>
    /// 處理項目
    /// </summary>
    /// <param name="itemId">項目ID</param>
    /// <returns>處理結果</returns>
    public async Task<string> ProcessItemAsync(int itemId)
    {
        await Task.Delay(100); // 模擬處理時間
        return $"Processed_{itemId}";
    }
}