using TUnit.Advanced.Core.Models;

namespace TUnit.Advanced.Core.Services;

/// <summary>
/// 資料存取服務介面
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// 儲存訂單
    /// </summary>
    Task<bool> SaveOrderAsync(Order order);

    /// <summary>
    /// 根據編號取得訂單
    /// </summary>
    Task<Order?> GetOrderByIdAsync(string orderId);

    /// <summary>
    /// 更新訂單
    /// </summary>
    Task<bool> UpdateOrderAsync(Order order);

    /// <summary>
    /// 刪除訂單
    /// </summary>
    Task<bool> DeleteOrderAsync(string orderId);

    /// <summary>
    /// 根據客戶取得訂單清單
    /// </summary>
    Task<List<Order>> GetOrdersByCustomerIdAsync(string customerId);
}

/// <summary>
/// 折扣規則資料存取介面
/// </summary>
public interface IDiscountRepository
{
    /// <summary>
    /// 根據折扣碼取得規則
    /// </summary>
    Task<DiscountRule?> GetDiscountRuleByCodeAsync(string discountCode);

    /// <summary>
    /// 取得有效的折扣規則清單
    /// </summary>
    Task<List<DiscountRule>> GetActiveDiscountRulesAsync();
}