namespace Day11.Core;

/// <summary>
/// class Product - 產品類別，包含多種數值型別屬性用於展示泛型範圍控制
/// </summary>
public class Product
{
    /// <summary>
    /// 產品名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 產品價格（decimal 型別）
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 庫存數量（int 型別）
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// 評分（double 型別）
    /// </summary>
    public double Rating { get; set; }

    /// <summary>
    /// 折扣（float 型別）
    /// </summary>
    public float Discount { get; set; }
}