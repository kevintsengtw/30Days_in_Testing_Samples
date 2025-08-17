using System;

namespace Day09.Core.Models;

/// <summary>
/// class PaymentRequest - 付款請求
/// </summary>
public class PaymentRequest
{
    /// <summary>
    /// 付款金額
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 付款方式
    /// </summary>
    public PaymentMethod PaymentMethod { get; set; }
}