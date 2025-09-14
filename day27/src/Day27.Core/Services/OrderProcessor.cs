using Day27.Core.Interfaces;
using Day27.Core.Models;

namespace Day27.Core.Services;

/// <summary>
/// 訂單處理服務
/// </summary>
public class OrderProcessor
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly IInventoryService _inventoryService;
    private readonly INotificationService _notificationService;

    public OrderProcessor(
        IOrderRepository orderRepository,
        IPaymentService paymentService,
        IInventoryService inventoryService,
        INotificationService notificationService)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    /// <summary>
    /// 處理訂單
    /// </summary>
    /// <param name="order">訂單資料</param>
    /// <returns>處理後的訂單</returns>
    /// <exception cref="ArgumentNullException">當訂單為 null</exception>
    /// <exception cref="InvalidOperationException">當庫存不足或付款失敗</exception>
    public async Task<Order> ProcessOrderAsync(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        // 1. 檢查庫存
        var hasStock = await _inventoryService.CheckAvailabilityAsync(order.ProductId, order.Quantity);
        if (!hasStock)
        {
            throw new InvalidOperationException("庫存不足");
        }

        // 2. 保留庫存
        var stockReserved = await _inventoryService.ReserveStockAsync(order.ProductId, order.Quantity);
        if (!stockReserved)
        {
            throw new InvalidOperationException("無法保留庫存");
        }

        try
        {
            // 3. 處理付款
            var paymentResult = await _paymentService.ProcessPaymentAsync(order.TotalAmount, order.PaymentMethod);
            if (!paymentResult.Success)
            {
                throw new InvalidOperationException($"付款失敗：{paymentResult.ErrorMessage}");
            }

            // 4. 更新訂單狀態
            order.Status = OrderStatus.Confirmed;
            var savedOrder = await _orderRepository.SaveAsync(order);

            // 5. 發送確認通知
            await _notificationService.SendOrderConfirmationAsync(order.CustomerId, order.Id.ToString());

            return savedOrder;
        }
        catch
        {
            // 如果處理失敗，這裡應該要釋放已保留的庫存
            // 為了簡化範例，這裡省略實作
            throw;
        }
    }

    /// <summary>
    /// 取消訂單
    /// </summary>
    /// <param name="orderId">訂單識別碼</param>
    /// <returns>取消後的訂單</returns>
    /// <exception cref="ArgumentException">當訂單不存在</exception>
    /// <exception cref="InvalidOperationException">當訂單狀態不允許取消</exception>
    public async Task<Order> CancelOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order is null)
        {
            throw new ArgumentException("訂單不存在", nameof(orderId));
        }

        if (order.Status == OrderStatus.Paid)
        {
            throw new InvalidOperationException("已付款的訂單無法取消");
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("訂單已被取消");
        }

        order.Status = OrderStatus.Cancelled;
        return await _orderRepository.SaveAsync(order);
    }
}