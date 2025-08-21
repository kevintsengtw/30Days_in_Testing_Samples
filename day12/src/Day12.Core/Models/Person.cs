using System.ComponentModel.DataAnnotations;

namespace Day12.Core.Models;

/// <summary>
/// class Person - 人員基本資料
/// </summary>
public class Person
{
    /// <summary>
    /// 唯一識別碼
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 姓名（最大長度10字元）
    /// </summary>
    [StringLength(10)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 年齡（18-80歲）
    /// </summary>
    [Range(18, 80)]
    public int Age { get; set; }

    /// <summary>
    /// 電子郵件地址
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreateTime { get; set; }
}