using Day08.Core.Interface;
using Day08.Core.Models;

namespace Day08.Core.Services;

/// <summary>
/// class OrderProcessingService - 訂單處理服務
/// </summary>
public class OrderProcessingService
{
    private readonly ILogger<OrderProcessingService> _logger;
    private readonly IInventoryService _inventoryService;
    private readonly IPaymentService _paymentService;

    /// <summary>
    /// OrderProcessingService 建構子
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="inventoryService">The inventoryService.</param>
    /// <param name="paymentService">The paymentService.</param>
    public OrderProcessingService(
        ILogger<OrderProcessingService> logger,
        IInventoryService inventoryService,
        IPaymentService paymentService)
    {
        this._logger = logger;
        this._inventoryService = inventoryService;
        this._paymentService = paymentService;
    }

    /// <summary>
    /// 處理訂單
    /// </summary>
    /// <param name="order">訂單資訊</param>
    /// <returns>處理結果</returns>
    public OrderResult ProcessOrder(Order order)
    {
        this._logger.LogInformation("開始處理訂單 {OrderId} for customer {CustomerId}", order.Id, order.CustomerId);

        // 檢查庫存
        var stockAvailable = this._inventoryService.CheckStock(order.ProductId, order.Quantity);
        if (!stockAvailable)
        {
            this._logger.LogWarning("商品 {ProductId} 庫存不足，數量需求：{RequestedQuantity}",
                                    order.ProductId, order.Quantity);
            return new OrderResult { Success = false, ErrorMessage = "庫存不足" };
        }

        // 處理付款
        var paymentResult = this._paymentService.ProcessPayment(order.TotalAmount);
        if (!paymentResult.Success)
        {
            this._logger.LogError("訂單 {OrderId} 付款失敗：{ErrorMessage}",
                                  order.Id, paymentResult.ErrorMessage);
            return new OrderResult { Success = false, ErrorMessage = "付款失敗" };
        }

        this._logger.LogInformation("訂單 {OrderId} 處理完成，金額：{Amount}", order.Id, order.TotalAmount);
        return new OrderResult { Success = true, OrderId = order.Id };
    }
}