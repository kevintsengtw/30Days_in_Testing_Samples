using TUnit.Advanced.Core.Models;

namespace TUnit.Advanced.Core.Services;

/// <summary>
/// 訂單服務介面
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// 建立新訂單
    /// </summary>
    Task<Order> CreateOrderAsync(string customerId, CustomerLevel customerLevel, List<OrderItem> items);

    /// <summary>
    /// 根據編號取得訂單
    /// </summary>
    Task<Order?> GetOrderByIdAsync(string orderId);

    /// <summary>
    /// 更新訂單狀態
    /// </summary>
    Task<bool> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus);

    /// <summary>
    /// 取消訂單
    /// </summary>
    Task<bool> CancelOrderAsync(string orderId);

    /// <summary>
    /// 套用折扣碼
    /// </summary>
    Task<bool> ApplyDiscountAsync(string orderId, string discountCode);
}