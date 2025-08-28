using Day19.WebApplication.Controllers.Examples.Level2;

namespace Day19.WebApplication.Services;

/// <summary>
/// 訂單服務的簡單實作 (用於 Level 2 範例)
/// </summary>
public class OrderService : IOrderService
{
    private static readonly List<Order> _orders = new();
    private static int _nextId = 1;
    private readonly TimeProvider _timeProvider;

    public OrderService(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    public Task<Order?> GetOrderAsync(int orderId)
    {
        var order = _orders.FirstOrDefault(o => o.Id == orderId);
        return Task.FromResult(order);
    }

    public Task<Order> CreateOrderAsync(string productName, int quantity, string customerEmail)
    {
        var order = new Order
        {
            Id = _nextId++,
            ProductName = productName,
            Quantity = quantity,
            CustomerEmail = customerEmail,
            Status = "Pending",
            CreatedAt = _timeProvider.GetUtcNow().DateTime
        };

        _orders.Add(order);
        return Task.FromResult(order);
    }

    public Task CompleteOrderAsync(int orderId)
    {
        var order = _orders.FirstOrDefault(o => o.Id == orderId);
        if (order != null)
        {
            order.Status = "Completed";
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// 庫存服務的簡單實作 (用於 Level 2 範例)
/// </summary>
public class InventoryService : IInventoryService
{
    private static readonly Dictionary<string, Inventory> _inventory = new()
    {
        ["Test Product"] = new() { ProductName = "Test Product", AvailableQuantity = 100, Price = 29.99m },
        ["Limited Product"] = new() { ProductName = "Limited Product", AvailableQuantity = 5, Price = 99.99m }
    };

    public Task<bool> CheckStockAsync(string productName, int quantity)
    {
        if (_inventory.TryGetValue(productName, out var inventory))
        {
            return Task.FromResult(inventory.AvailableQuantity >= quantity);
        }

        return Task.FromResult(false);
    }

    public Task<Inventory?> GetInventoryAsync(string productName)
    {
        _inventory.TryGetValue(productName, out var inventory);
        return Task.FromResult(inventory);
    }
}

/// <summary>
/// 通知服務的簡單實作 (用於 Level 2 範例)
/// </summary>
public class NotificationService : INotificationService
{
    public Task SendOrderConfirmationAsync(int orderId, string customerEmail)
    {
        // 模擬發送確認通知
        Console.WriteLine($"Order confirmation sent to {customerEmail} for order {orderId}");
        return Task.CompletedTask;
    }

    public Task SendOrderCompletionAsync(int orderId, string customerEmail)
    {
        // 模擬發送完成通知
        Console.WriteLine($"Order completion notification sent to {customerEmail} for order {orderId}");
        return Task.CompletedTask;
    }
}