namespace Day19.WebApplication.Models;

/// <summary>
/// 建立貨運商參數
/// </summary>
public class ShipperCreateParameter
{
    /// <summary>
    /// 公司名稱
    /// </summary>
    [Required(ErrorMessage = "公司名稱為必填")]
    [StringLength(40, ErrorMessage = "公司名稱不可超過 40 個字元")]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// 聯絡電話
    /// </summary>
    [StringLength(24, ErrorMessage = "電話號碼不可超過 24 個字元")]
    public string Phone { get; set; } = string.Empty;
}