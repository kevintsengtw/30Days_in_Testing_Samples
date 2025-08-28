using Microsoft.AspNetCore.Mvc;

namespace Day19.WebApplication.Controllers.Examples.Level2;

/// <summary>
/// Level 2 整合測試控制器：包含服務依賴的 API
/// 特色：使用服務抽象，可透過 Service Stub 進行測試
/// 測試重點：服務整合、錯誤處理、業務邏輯驗證
/// </summary>
[ApiController]
[Route("api/v2/[controller]")]
public class ServiceDependentController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IInventoryService _inventoryService;
    private readonly INotificationService _notificationService;

    public ServiceDependentController(
        IOrderService orderService,
        IInventoryService inventoryService,
        INotificationService notificationService)
    {
        _orderService = orderService;
        _inventoryService = inventoryService;
        _notificationService = notificationService;
    }

    /// <summary>
    /// 取得訂單資訊
    /// </summary>
    [HttpGet("orders/{orderId}")]
    public async Task<IActionResult> GetOrder(int orderId)
    {
        if (orderId <= 0)
        {
            return BadRequest(new { Error = "Order ID must be greater than 0" });
        }

        try
        {
            var order = await _orderService.GetOrderAsync(orderId);

            if (order == null)
            {
                return NotFound(new { Error = $"Order {orderId} not found" });
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
        }
    }

    /// <summary>
    /// 建立新訂單
    /// </summary>
    [HttpPost("orders")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.ProductName) || request.Quantity <= 0)
        {
            return BadRequest(new { Error = "Invalid order data" });
        }

        try
        {
            // 檢查庫存
            var hasStock = await _inventoryService.CheckStockAsync(request.ProductName, request.Quantity);
            if (!hasStock)
            {
                return BadRequest(new { Error = "Insufficient stock" });
            }

            // 建立訂單
            var order = await _orderService.CreateOrderAsync(request.ProductName, request.Quantity, request.CustomerEmail);

            // 發送通知
            await _notificationService.SendOrderConfirmationAsync(order.Id, request.CustomerEmail);

            return CreatedAtAction(nameof(GetOrder), new { orderId = order.Id }, order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Failed to create order", Details = ex.Message });
        }
    }

    /// <summary>
    /// 取得庫存狀態
    /// </summary>
    [HttpGet("inventory/{productName}")]
    public async Task<IActionResult> GetInventory(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            return BadRequest(new { Error = "Product name is required" });
        }

        try
        {
            var inventory = await _inventoryService.GetInventoryAsync(productName);

            if (inventory == null)
            {
                return NotFound(new { Error = $"Product {productName} not found" });
            }

            return Ok(inventory);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Failed to get inventory", Details = ex.Message });
        }
    }

    /// <summary>
    /// 處理訂單完成
    /// </summary>
    [HttpPost("orders/{orderId}/complete")]
    public async Task<IActionResult> CompleteOrder(int orderId)
    {
        if (orderId <= 0)
        {
            return BadRequest(new { Error = "Order ID must be greater than 0" });
        }

        try
        {
            var order = await _orderService.GetOrderAsync(orderId);
            if (order == null)
            {
                return NotFound(new { Error = $"Order {orderId} not found" });
            }

            if (order.Status == "Completed")
            {
                return BadRequest(new { Error = "Order is already completed" });
            }

            // 完成訂單
            await _orderService.CompleteOrderAsync(orderId);

            // 發送完成通知
            await _notificationService.SendOrderCompletionAsync(orderId, order.CustomerEmail);

            return Ok(new { Message = "Order completed successfully", OrderId = orderId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Failed to complete order", Details = ex.Message });
        }
    }
}

// Service 介面定義
public interface IOrderService
{
    Task<Order?> GetOrderAsync(int orderId);
    Task<Order> CreateOrderAsync(string productName, int quantity, string customerEmail);
    Task CompleteOrderAsync(int orderId);
}

public interface IInventoryService
{
    Task<bool> CheckStockAsync(string productName, int quantity);
    Task<Inventory?> GetInventoryAsync(string productName);
}

public interface INotificationService
{
    Task SendOrderConfirmationAsync(int orderId, string customerEmail);
    Task SendOrderCompletionAsync(int orderId, string customerEmail);
}

// 資料模型
public class Order
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Inventory
{
    public string ProductName { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
    public decimal Price { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class CreateOrderRequest
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
}