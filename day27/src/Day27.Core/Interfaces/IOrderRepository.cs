using Day27.Core.Models;

namespace Day27.Core.Interfaces;

/// <summary>
/// 訂單倉庫介面
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// 儲存訂單
    /// </summary>
    /// <param name="order">訂單實體</param>
    /// <returns>儲存後的訂單實體</returns>
    Task<Order> SaveAsync(Order order);

    /// <summary>
    /// 根據識別碼取得訂單
    /// </summary>
    /// <param name="id">訂單識別碼</param>
    /// <returns>訂單實體</returns>
    Task<Order?> GetByIdAsync(int id);
}