/// <summary>
/// 訂單服務
/// </summary>
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;

    public OrderService(IOrderRepository orderRepository, IUserRepository userRepository)
    {
        _orderRepository = orderRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// 處理訂單
    /// </summary>
    public async Task<Order> ProcessOrderAsync(string userId, List<OrderItem> items)
    {
        // 模擬耗時的處理過程
        await Task.Delay(100); // 這會讓測試變慢

        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
            throw new ArgumentException("使用者不存在");

        var order = new Order
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Items = items,
            CreateTime = DateTime.Now,
            TotalAmount = items.Sum(x => x.Price * x.Quantity)
        };

        await _orderRepository.SaveOrderAsync(order);
        return order;
    }

    /// <summary>
    /// 計算訂單總金額（包含複雜業務邏輯）
    /// </summary>
    public async Task<decimal> CalculateOrderTotalAsync(List<OrderItem> items, string? discountCode = null)
    {
        // 模擬複雜的計算邏輯
        var baseTotal = items.Sum(x => x.Price * x.Quantity);

        // 折扣計算
        decimal discount = 0;
        if (!string.IsNullOrEmpty(discountCode))
        {
            // 模擬查詢折扣資料庫（耗時操作）
            await Task.Delay(50);

            discount = discountCode switch
            {
                "SAVE10" => 0.1m,
                "SAVE20" => 0.2m,
                _ => 0
            };
        }

        return baseTotal * (1 - discount);
    }
}
