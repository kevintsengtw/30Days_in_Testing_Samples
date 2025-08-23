namespace Day14.Core.Models;

/// <summary>
/// 台灣人員資訊
/// </summary>
public class TaiwanPerson
{
    /// <summary>
    /// 編號
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// 城市
    /// </summary>
    public string City { get; set; } = "";

    /// <summary>
    /// 大學
    /// </summary>
    public string University { get; set; } = "";

    /// <summary>
    /// 公司
    /// </summary>
    public string Company { get; set; } = "";

    /// <summary>
    /// 身分證字號
    /// </summary>
    public string IdCard { get; set; } = "";

    /// <summary>
    /// 手機號碼
    /// </summary>
    public string Mobile { get; set; } = "";
}