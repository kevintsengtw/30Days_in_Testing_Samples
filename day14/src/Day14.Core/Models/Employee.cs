namespace Day14.Core.Models;

/// <summary>
/// 員工資訊
/// </summary>
public class Employee
{
    /// <summary>
    /// 員工編號
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 員工代碼
    /// </summary>
    public string EmployeeId { get; set; } = "";

    /// <summary>
    /// 姓
    /// </summary>
    public string FirstName { get; set; } = "";

    /// <summary>
    /// 名
    /// </summary>
    public string LastName { get; set; } = "";

    /// <summary>
    /// 電子郵件
    /// </summary>
    public string Email { get; set; } = "";

    /// <summary>
    /// 年齡
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// 職級
    /// </summary>
    public string Level { get; set; } = "";

    /// <summary>
    /// 薪資
    /// </summary>
    public decimal Salary { get; set; }

    /// <summary>
    /// 入職日期
    /// </summary>
    public DateTime HireDate { get; set; }

    /// <summary>
    /// 部門
    /// </summary>
    public string Department { get; set; } = "";

    /// <summary>
    /// 技能
    /// </summary>
    public List<string> Skills { get; set; } = new();

    /// <summary>
    /// 專案經驗
    /// </summary>
    public List<Project> Projects { get; set; } = new();
}