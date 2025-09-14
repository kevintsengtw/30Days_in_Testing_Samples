/// <summary>
/// 訂單資料模型
/// </summary>
public class Order
{
    /// <summary>
    /// 訂單識別碼
    /// </summary>
    public string Id { get; set; } = "";

    /// <summary>
    /// 使用者識別碼
    /// </summary>
    public string UserId { get; set; } = "";

    /// <summary>
    /// 訂單項目清單
    /// </summary>
    public List<OrderItem> Items { get; set; } = new();

    /// <summary>
    /// 訂單總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreateTime { get; set; }
}

/// <summary>
/// 訂單項目
/// </summary>
public class OrderItem
{
    /// <summary>
    /// 商品識別碼
    /// </summary>
    public string ProductId { get; set; } = "";

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string ProductName { get; set; } = "";

    /// <summary>
    /// 單價
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    public int Quantity { get; set; }
}
