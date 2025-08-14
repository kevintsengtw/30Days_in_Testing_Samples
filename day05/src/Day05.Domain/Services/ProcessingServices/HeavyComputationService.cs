using Day05.Domain.DomainModels;

namespace Day05.Domain.Services.ProcessingServices;

/// <summary>
/// class HeavyComputationService - 重量級計算服務
/// </summary>
public class HeavyComputationService
{
    /// <summary>
    /// 處理大型數據集
    /// </summary>
    /// <param name="dataSet">數據集</param>
    /// <returns>處理結果</returns>
    public string ProcessLargeDataSet(List<DataRecord> dataSet)
    {
        // 模擬重量級計算
        Thread.Sleep(1000);
        return $"Processed {dataSet.Count} items";
    }
}