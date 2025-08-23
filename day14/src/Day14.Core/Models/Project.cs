namespace Day14.Core.Models;

/// <summary>
/// 專案資訊
/// </summary>
public class Project
{
    /// <summary>
    /// 專案編號
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 專案名稱
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// 專案描述
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// 使用技術
    /// </summary>
    public List<string> Technologies { get; set; } = new();
}