using Day10.Core.Enums;
using Day10.Core.Models;

namespace Day10.Core.Services;

/// <summary>
/// 訂單處理器
/// </summary>
public class OrderProcessor
{
    /// <summary>
    /// 處理訂單
    /// </summary>
    public ProcessResult Process(Order order)
    {
        if (order == null)
        {
            return new ProcessResult { Success = false, Message = "訂單不能為 null" };
        }

        if (order.Status == OrderStatus.Completed)
        {
            return new ProcessResult
            {
                Success = true,
                OrderId = order.Id,
                TotalAmount = order.CalculateTotal()
            };
        }

        return new ProcessResult { Success = false, Message = "訂單狀態不正確" };
    }
}