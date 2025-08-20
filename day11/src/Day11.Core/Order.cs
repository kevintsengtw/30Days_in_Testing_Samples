namespace Day11.Core;

/// <summary>
/// class Order - 訂單類別，包含多種數值型別屬性用於展示泛型範圍控制
/// </summary>
public class Order
{
    /// <summary>
    /// 訂單編號
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// 訂單金額（decimal 型別）
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 商品數量（short 型別）
    /// </summary>
    public short ItemCount { get; set; }

    /// <summary>
    /// 優先順序（byte 型別）
    /// </summary>
    public byte Priority { get; set; }

    /// <summary>
    /// 客戶 ID（long 型別）
    /// </summary>
    public long CustomerId { get; set; }
}