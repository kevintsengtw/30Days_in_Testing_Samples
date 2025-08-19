namespace Day10.Core.Services;

/// <summary>
/// 資料處理器
/// </summary>
public class DataProcessor
{
    /// <summary>
    /// 批次處理資料
    /// </summary>
    public ProcessingResult ProcessBatch(IEnumerable<DataRecord> records)
    {
        var processed = 0;
        var errors = new List<string>();

        foreach (var record in records)
        {
            try
            {
                // 處理邏輯...
                processed++;
            }
            catch (Exception ex)
            {
                errors.Add($"Record {record.Id}: {ex.Message}");
            }
        }

        return new ProcessingResult
        {
            ProcessedCount = processed,
            ErrorCount = errors.Count,
            Errors = errors
        };
    }
}