using Day08.Core.Models;

namespace Day08.Core.Interface;

/// <summary>
/// interface IOrderRepository - 訂單儲存庫介面
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// 儲存訂單
    /// </summary>
    /// <param name="order">訂單資訊</param>
    /// <returns>是否儲存成功</returns>
    Task<bool> SaveOrderAsync(Order order);

    /// <summary>
    /// 根據編號取得訂單
    /// </summary>
    /// <param name="orderId">訂單編號</param>
    /// <returns>訂單資訊</returns>
    Task<Order?> GetOrderByIdAsync(string orderId);
}