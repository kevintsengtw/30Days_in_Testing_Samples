using TUnit.Advanced.Core.Models;

namespace TUnit.Advanced.Core.Services;

/// <summary>
/// 訂單服務實作
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IDiscountCalculator _discountCalculator;
    private readonly IShippingCalculator _shippingCalculator;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IDiscountCalculator discountCalculator,
        IShippingCalculator shippingCalculator,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _discountCalculator = discountCalculator;
        _shippingCalculator = shippingCalculator;
        _logger = logger;
    }

    /// <summary>
    /// 建立新訂單
    /// </summary>
    /// <param name="customerId">customerId</param>
    /// <param name="customerLevel">customerLevel</param>
    /// <param name="items">items</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Order> CreateOrderAsync(string customerId, CustomerLevel customerLevel, List<OrderItem> items)
    {
        if (string.IsNullOrWhiteSpace(customerId))
        {
            throw new ArgumentException("客戶識別碼不能為空", nameof(customerId));
        }

        if (items == null || items.Count == 0)
        {
            throw new ArgumentException("訂單項目不能為空", nameof(items));
        }

        if (items.Any(item => item.Quantity <= 0))
        {
            throw new ArgumentException("訂單項目數量必須大於零");
        }

        var order = new Order
        {
            OrderId = GenerateOrderId(),
            CustomerId = customerId,
            CustomerLevel = customerLevel,
            Items = items,
            Status = OrderStatus.待處理,
            CreatedAt = DateTime.UtcNow
        };

        // 計算運費
        order.ShippingFee = _shippingCalculator.CalculateShippingFee(order);

        var success = await _orderRepository.SaveOrderAsync(order);
        if (!success)
        {
            throw new InvalidOperationException("訂單建立失敗");
        }

        _logger.LogInformation("成功建立訂單 {OrderId}，客戶: {CustomerId}", order.OrderId, customerId);
        return order;
    }

    /// <summary>
    /// 根據編號取得訂單
    /// </summary>
    /// <param name="orderId">orderId</param>
    /// <returns></returns>
    public async Task<Order?> GetOrderByIdAsync(string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId))
        {
            return null;
        }

        return await _orderRepository.GetOrderByIdAsync(orderId);
    }

    /// <summary>
    /// 更新訂單狀態
    /// </summary>
    /// <param name="orderId">orderId</param>
    /// <param name="newStatus">newStatus</param>
    /// <returns></returns>
    public async Task<bool> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            _logger.LogWarning("嘗試更新不存在的訂單狀態: {OrderId}", orderId);
            return false;
        }

        if (!IsValidStatusTransition(order.Status, newStatus))
        {
            _logger.LogWarning("無效的訂單狀態轉換: {OldStatus} -> {NewStatus}", order.Status, newStatus);
            return false;
        }

        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        var success = await _orderRepository.UpdateOrderAsync(order);
        if (success)
        {
            _logger.LogInformation("訂單 {OrderId} 狀態更新為 {Status}", orderId, newStatus);
        }

        return success;
    }

    /// <summary>
    /// 取消訂單
    /// </summary>
    /// <param name="orderId">orderId</param>
    /// <returns></returns>
    public async Task<bool> CancelOrderAsync(string orderId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return false;
        }

        if (order.Status is OrderStatus.已發貨 or OrderStatus.已完成)
        {
            _logger.LogWarning("無法取消已發貨或已完成的訂單: {OrderId}", orderId);
            return false;
        }

        return await UpdateOrderStatusAsync(orderId, OrderStatus.已取消);
    }

    /// <summary>
    /// 套用折扣碼
    /// </summary>
    /// <param name="orderId">orderId</param>
    /// <param name="discountCode">discountCode</param>
    /// <returns></returns>
    public async Task<bool> ApplyDiscountAsync(string orderId, string discountCode)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return false;
        }

        if (order.Status != OrderStatus.待處理)
        {
            _logger.LogWarning("只能對待處理的訂單套用折扣碼: {OrderId}", orderId);
            return false;
        }

        var discountAmount = await _discountCalculator.CalculateDiscountAsync(order, discountCode);
        if (discountAmount <= 0)
        {
            return false;
        }

        order.DiscountCode = discountCode;
        order.DiscountAmount = discountAmount;
        order.UpdatedAt = DateTime.UtcNow;

        return await _orderRepository.UpdateOrderAsync(order);
    }

    /// <summary>
    /// 產生唯一訂單編號
    /// </summary>
    /// <returns></returns>
    private static string GenerateOrderId()
    {
        return $"ORD{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
    }

    /// <summary>
    /// 檢查訂單狀態轉換是否有效
    /// </summary>
    /// <param name="currentStatus">currentStatus</param>
    /// <param name="newStatus">newStatus</param>
    /// <returns></returns>
    private static bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        return currentStatus switch
        {
            OrderStatus.待處理 => newStatus is OrderStatus.已確認 or OrderStatus.已取消,
            OrderStatus.已確認 => newStatus is OrderStatus.處理中 or OrderStatus.已取消,
            OrderStatus.處理中 => newStatus is OrderStatus.已發貨 or OrderStatus.已取消,
            OrderStatus.已發貨 => newStatus == OrderStatus.已完成,
            OrderStatus.已完成 => false,
            OrderStatus.已取消 => false,
            _ => false
        };
    }
}