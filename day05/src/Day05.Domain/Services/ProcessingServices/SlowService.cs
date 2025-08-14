namespace Day05.Domain.Services.ProcessingServices;

/// <summary>
/// class SlowService - 緩慢服務
/// </summary>
public class SlowService
{
    /// <summary>
    /// 處理慢速任務
    /// </summary>
    public async Task ProcessAsync()
    {
        await Task.Delay(2000); // 模擬耗時操作
    }
}