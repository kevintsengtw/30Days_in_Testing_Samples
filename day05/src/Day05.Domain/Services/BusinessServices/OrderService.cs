using Day05.Domain.DomainModels;

namespace Day05.Domain.Services.BusinessServices;

/// <summary>
/// class OrderService - 訂單服務
/// </summary>
public class OrderService
{
    /// <summary>
    /// 創建訂單
    /// </summary>
    /// <param name="items">訂單項目</param>
    /// <returns>創建的訂單</returns>
    public Order CreateOrder(OrderItem[] items)
    {
        return new Order
        {
            Id = Random.Shared.Next(1, 1000),
            CustomerName = "Test Customer",
            TotalAmount = items.Sum(i => i.Price * i.Quantity),
            Status = "Pending",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            Items = items,
            AuditInfo = new AuditInfo
            {
                CreatedBy = "system",
                CreatedAt = DateTime.Now,
                ModifiedBy = "system",
                ModifiedAt = DateTime.Now
            }
        };
    }

    /// <summary>
    /// 處理訂單
    /// </summary>
    /// <param name="orderId">訂單ID</param>
    /// <returns>處理後的訂單</returns>
    public Order ProcessOrder(int orderId)
    {
        // 模擬處理邏輯
        Thread.Sleep(100);

        return new Order
        {
            Id = orderId,
            CustomerName = "Test Customer",
            TotalAmount = 100.0m,
            Status = "Processing",
            CreatedAt = DateTime.Now.AddHours(-1),
            UpdatedAt = DateTime.Now,
            ProcessedAt = DateTime.Now,
            Items = [],
            AuditInfo = new AuditInfo
            {
                CreatedBy = "system",
                CreatedAt = DateTime.Now.AddHours(-1),
                ModifiedBy = "processor",
                ModifiedAt = DateTime.Now
            }
        };
    }

    /// <summary>
    /// 獲取訂單
    /// </summary>
    /// <param name="id">訂單ID</param>
    /// <returns>訂單</returns>
    public Order GetOrder(int id)
    {
        return new Order
        {
            Id = id,
            CustomerName = "John Doe",
            TotalAmount = 100.0m,
            Status = "Completed",
            CreatedAt = DateTime.Now.AddDays(-1),
            UpdatedAt = DateTime.Now,
            Items =
            [
                new OrderItem
                {
                    Id = 1,
                    ProductName = "Laptop",
                    Price = 100.0m,
                    Quantity = 1,
                    AddedAt = DateTime.Now.AddDays(-1),
                    ModifiedAt = DateTime.Now
                }
            ],
            AuditInfo = new AuditInfo
            {
                CreatedBy = "system",
                CreatedAt = DateTime.Now.AddDays(-1),
                ModifiedBy = "system",
                ModifiedAt = DateTime.Now
            }
        };
    }

    /// <summary>
    /// 批量處理訂單
    /// </summary>
    /// <param name="orders">訂單列表</param>
    /// <returns>處理後的訂單列表</returns>
    public List<Order> ProcessOrderBatch(List<Order> orders)
    {
        return orders.Select(o => new Order
        {
            Id = o.Id,
            CustomerName = o.CustomerName,
            TotalAmount = o.TotalAmount,
            Status = "Processed",
            CreatedAt = o.CreatedAt,
            UpdatedAt = DateTime.Now,
            ProcessedAt = DateTime.Now,
            Items = o.Items
        }).ToList();
    }
}