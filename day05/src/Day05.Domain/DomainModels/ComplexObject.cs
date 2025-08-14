namespace Day05.Domain.DomainModels;

/// <summary>
/// class ComplexObject - 複雜物件
/// </summary>
public class ComplexObject
{
    /// <summary>
    /// 重要屬性1
    /// </summary>
    public string ImportantProperty1 { get; set; } = string.Empty;

    /// <summary>
    /// 重要屬性2
    /// </summary>
    public string ImportantProperty2 { get; set; } = string.Empty;

    /// <summary>
    /// 關鍵資料
    /// </summary>
    public string CriticalData { get; set; } = string.Empty;

    /// <summary>
    /// 時間戳記
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 產生的識別碼
    /// </summary>
    public Guid GeneratedId { get; set; }
}