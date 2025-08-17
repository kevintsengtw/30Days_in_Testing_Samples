using System;

namespace Day09.Core.Models;

/// <summary>
/// class PaymentResult - 付款結果
/// </summary>
public class PaymentResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 總金額（包含手續費）
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// 建立成功結果
    /// </summary>
    /// <param name="totalAmount">總金額</param>
    /// <returns>成功的付款結果</returns>
    public static PaymentResult Success(decimal totalAmount)
    {
        return new PaymentResult
        {
            IsSuccess = true,
            TotalAmount = totalAmount
        };
    }

    /// <summary>
    /// 建立失敗結果
    /// </summary>
    /// <param name="errorMessage">錯誤訊息</param>
    /// <returns>失敗的付款結果</returns>
    public static PaymentResult Failed(string errorMessage)
    {
        return new PaymentResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage
        };
    }
}