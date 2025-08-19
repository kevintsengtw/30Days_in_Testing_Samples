using Day10.Core.Enums;
using Day10.Core.Models;

namespace Day10.Core.Services;

/// <summary>
/// 折扣計算器
/// </summary>
public class DiscountCalculator
{
    /// <summary>
    /// 計算折扣
    /// </summary>
    public decimal Calculate(Customer customer, Order? order = null)
    {
        if (customer == null)
        {
            return 0;
        }

        return customer.Type switch
        {
            CustomerType.Regular => 0,
            CustomerType.Premium => 0.05m,
            CustomerType.VIP => 0.15m,
            _ => 0
        };
    }

    /// <summary>
    /// 計算折扣
    /// </summary>
    public decimal Calculate(Customer customer)
    {
        if (customer == null)
        {
            return 0;
        }

        if (customer.TotalSpent >= 10000)
        {
            return 0.15m;
        }

        return 0;
    }
}