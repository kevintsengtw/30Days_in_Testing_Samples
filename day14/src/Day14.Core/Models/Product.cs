namespace Day14.Core.Models;

/// <summary>
/// 產品資訊
/// </summary>
public class Product
{
    /// <summary>
    /// 產品編號
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 產品名稱
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// 產品描述
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// 價格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 類別
    /// </summary>
    public string Category { get; set; } = "";

    /// <summary>
    /// 建立日期
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// 庫存數量
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 標籤
    /// </summary>
    public List<string> Tags { get; set; } = new();
}