using System;

namespace Day09.Core.Models;

/// <summary>
/// class PaymentMethod - 付款方式
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// 信用卡
    /// </summary>
    CreditCard,

    /// <summary>
    /// 簽帳卡
    /// </summary>
    DebitCard,

    /// <summary>
    /// 銀行轉帳
    /// </summary>
    BankTransfer,

    /// <summary>
    /// 現金
    /// </summary>
    Cash
}