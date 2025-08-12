namespace Day03.Core.Models;

/// <summary>
/// class UserSettings - 代表使用者設定。
/// </summary>
public class UserSettings
{
    /// <summary>
    /// 使用者介面主題。
    /// </summary>
    /// <value></value>
    public string Theme { get; set; } = "Light";

    /// <summary>
    /// 使用者語言設定。
    /// </summary>
    /// <value></value>
    public string Language { get; set; } = "en-US";
}