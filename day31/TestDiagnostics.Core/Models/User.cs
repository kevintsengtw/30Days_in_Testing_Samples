/// <summary>
/// 使用者資料模型
/// </summary>
public class User
{
    /// <summary>
    /// 使用者識別碼
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// 使用者名稱
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// 是否為活躍使用者
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 最後登入時間
    /// </summary>
    public DateTime LastLoginTime { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreateTime { get; set; }
}
