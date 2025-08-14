namespace Day05.Domain.DomainModels;

/// <summary>
/// class AuditInfo - 審計資訊
/// </summary>
public class AuditInfo
{
    /// <summary>
    /// 建立者
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 修改者
    /// </summary>
    public string ModifiedBy { get; set; } = string.Empty;

    /// <summary>
    /// 修改時間
    /// </summary>
    public DateTime ModifiedAt { get; set; }
}