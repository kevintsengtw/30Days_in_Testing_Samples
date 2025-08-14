namespace Day05.Domain.DomainModels;

/// class DataRecord - 資料記錄
/// </summary>
public class DataRecord
{
    /// <summary>
    /// 資料識別碼
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 資料值
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 是否已處理
    /// </summary>
    public bool IsProcessed { get; set; }
}