namespace Day25.Api.Configuration;

/// <summary>
/// API 配置設定
/// </summary>
public class ApiConfiguration
{
    /// <summary>
    /// API 名稱
    /// </summary>
    public string Name { get; set; } = "Day25 Product API";

    /// <summary>
    /// API 版本
    /// </summary>
    public string Version { get; set; } = "v1";

    /// <summary>
    /// API 描述
    /// </summary>
    public string Description { get; set; } = "使用 .NET Aspire Testing 的產品管理 API";
}