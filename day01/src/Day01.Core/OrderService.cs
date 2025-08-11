using Day01.Core.Enums;
using Day01.Core.Models;

namespace Day01.Core;

/// <summary>
/// class OrderService - 訂單服務類別
/// </summary>
public class OrderService
{
    /// <summary>
    /// 處理訂單並產生訂單號碼
    /// </summary>
    /// <param name="order">要處理的訂單</param>
    /// <returns>包含處理結果的訂單物件</returns>
    public Order ProcessOrder(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        // 產生訂單號碼
        var processedOrder = new Order
        {
            Prefix = order.Prefix,
            Number = order.Number,
            Amount = order.Amount,
            Status = OrderStatus.Processed
        };

        return processedOrder;
    }

    /// <summary>
    /// 取得完整的訂單號碼
    /// </summary>
    /// <param name="order">訂單物件</param>
    /// <returns>格式化的訂單號碼</returns>
    public string GetOrderNumber(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        return $"{order.Prefix}-{order.Number}";
    } 
}