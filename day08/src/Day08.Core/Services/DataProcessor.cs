namespace Day08.Core.Services;

/// <summary>
/// class DataProcessor - 資料處理器（用於效能測試範例）
/// </summary>
public class DataProcessor
{
    private readonly ILogger<DataProcessor>? _logger;
    private readonly List<string> _dataSet;

    /// <summary>
    /// DataProcessor 建構子
    /// </summary>
    /// <param name="logger">The logger.</param>
    public DataProcessor(ILogger<DataProcessor>? logger = null)
    {
        this._logger = logger;
        this._dataSet = new List<string>();
    }

    /// <summary>
    /// 載入資料
    /// </summary>
    /// <param name="data">資料集合</param>
    /// <returns>非同步任務</returns>
    public async Task LoadData(IEnumerable<string> data)
    {
        this._logger?.LogInformation("開始載入資料，共 {Count} 筆", data.Count());

        await Task.Delay(100); // 模擬 I/O 操作

        this._dataSet.Clear();
        this._dataSet.AddRange(data);

        this._logger?.LogInformation("資料載入完成");
    }

    /// <summary>
    /// 驗證資料
    /// </summary>
    /// <returns>非同步任務</returns>
    public async Task ValidateData()
    {
        this._logger?.LogInformation("開始驗證資料");

        await Task.Delay(200); // 模擬驗證操作

        var invalidCount = this._dataSet.Count(data => string.IsNullOrWhiteSpace(data));
        if (invalidCount > 0)
        {
            this._logger?.LogWarning("發現 {InvalidCount} 筆無效資料", invalidCount);
        }

        this._logger?.LogInformation("資料驗證完成");
    }

    /// <summary>
    /// 處理資料
    /// </summary>
    /// <returns>處理結果</returns>
    public async Task<DataProcessingResult> ProcessData()
    {
        this._logger?.LogInformation("開始處理資料");

        await Task.Delay(300); // 模擬處理操作

        var result = new DataProcessingResult
        {
            ProcessedCount = this._dataSet.Count,
            Success = true
        };

        this._logger?.LogInformation("資料處理完成，處理 {ProcessedCount} 筆資料", result.ProcessedCount);

        return result;
    }
}