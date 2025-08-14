using Day05.Domain.DomainModels;

namespace Day05.Domain.Services.ProcessingServices;

/// <summary>
/// class DataProcessor - 數據處理器
/// </summary>
public class DataProcessor
{
    /// <summary>
    /// 處理大型數據集
    /// </summary>
    /// <param name="dataset">數據集</param>
    /// <returns>處理後的數據集</returns>
    public List<DataRecord> ProcessLargeDataset(List<DataRecord> dataset)
    {
        return dataset.Select(d => new DataRecord
        {
            Id = d.Id,
            Value = d.Value,
            IsProcessed = true
        }).ToList();
    }
}