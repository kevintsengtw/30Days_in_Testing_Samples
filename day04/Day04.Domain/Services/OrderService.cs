using Day04.Domain.Models;

namespace Day04.Domain.Services;

/// <summary>
/// class OrderService - 用於處理訂單的服務。
/// </summary>
public class OrderService
{
    private static readonly List<Order> Orders = [];
    private static int _nextId = 1;

    /// <summary>
    /// 創建訂單
    /// </summary>
    /// <param name="items">訂單項目列表</param>
    /// <returns>創建的訂單</returns>
    public Order CreateOrder(List<OrderItem> items)
    {
        if (items == null || items.Count == 0)
        {
            throw new ArgumentException("訂單必須包含至少一個項目", nameof(items));
        }

        var order = new Order
        {
            Id = _nextId++,
            Items = items,
            OrderDate = DateTime.UtcNow
        };

        Orders.Add(order);
        return order;
    }

    /// <summary>
    /// 獲取訂單
    /// </summary>
    /// <param name="id">訂單ID</param>
    /// <returns>找到的訂單</returns>
    public Order GetOrder(int id)
    {
        return Orders.FirstOrDefault(o => o.Id == id)
               ?? throw new InvalidOperationException($"找不到ID為 {id} 的訂單");
    }

    /// <summary>
    /// 處理訂單
    /// </summary>
    /// <param name="items">訂單項目列表</param>
    /// <returns>處理的訂單</returns>
    public Order ProcessOrder(List<OrderItem> items)
    {
        if (items.Any(item => item.Price <= 0))
        {
            throw new ArgumentException("所有項目的價格必須為正數");
        }

        if (items.Any(item => item.Quantity <= 0))
        {
            throw new ArgumentException("所有項目的數量必須為正數");
        }

        return this.CreateOrder(items);
    }

    /// <summary>
    /// 清空所有訂單
    /// </summary>
    public void ClearOrders()
    {
        Orders.Clear();
        _nextId = 1;
    }
}