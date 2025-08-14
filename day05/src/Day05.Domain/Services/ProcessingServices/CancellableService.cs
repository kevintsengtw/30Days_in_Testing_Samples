namespace Day05.Domain.Services.ProcessingServices;

/// <summary>
/// class CancellableService - 可取消服務
/// </summary>
public class CancellableService
{
    /// <summary>
    /// 執行可取消的長時間操作
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    public async Task LongRunningOperationAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(5000, cancellationToken); // 模擬長時間操作
    }
}