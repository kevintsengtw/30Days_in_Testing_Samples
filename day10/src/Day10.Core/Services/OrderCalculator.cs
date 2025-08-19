using Day10.Core.Models;

namespace Day10.Core.Services;

/// <summary>
/// 訂單計算器
/// </summary>
public class OrderCalculator
{
    /// <summary>
    /// 計算總金額
    /// </summary>
    public decimal CalculateTotal(Order order)
    {
        if (order?.Items == null)
        {
            return 0;
        }

        return order.Items.Sum(item => item.UnitPrice * item.Quantity);
    }
}