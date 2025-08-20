namespace Day11.Core;

/// <summary>
/// class Person - 人員類別，包含各種 DataAnnotation 屬性
/// </summary>
public class Person
{
    /// <summary>
    /// 唯一識別碼
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 姓名，限制長度為 10
    /// </summary>
    [StringLength(10)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 年齡，限制範圍為 10-80
    /// </summary>
    [Range(10, 80)]
    public int Age { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreateTime { get; set; }
}