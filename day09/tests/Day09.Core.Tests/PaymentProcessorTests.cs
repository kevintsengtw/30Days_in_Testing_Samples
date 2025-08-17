using System;
using Day09.Core.Models;
using Day09.Core.Tests.Helpers;

namespace Day09.Core.Tests;

/// <summary>
/// PaymentProcessor 測試類別（測試私有方法）
/// </summary>
public class PaymentProcessorTests
{
    [Fact]
    public void ProcessPayment_正常請求_應回傳成功結果()
    {
        // Arrange
        var processor = new PaymentProcessor();
        var request = new PaymentRequest
        {
            Amount = 1000m,
            PaymentMethod = PaymentMethod.CreditCard
        };

        // Act
        var actual = processor.ProcessPayment(request);

        // Assert
        actual.IsSuccess.Should().BeTrue();
        actual.TotalAmount.Should().Be(1030m); // 1000 + 3% fee
    }

    [Fact]
    public void ProcessPayment_無效請求_應回傳失敗結果()
    {
        // Arrange
        var processor = new PaymentProcessor();
        PaymentRequest? request = null;

        // Act
        var actual = processor.ProcessPayment(request!);

        // Assert
        actual.IsSuccess.Should().BeFalse();
        actual.ErrorMessage.Should().Be("Invalid request");
    }

    [Theory]
    [InlineData(1000, PaymentMethod.CreditCard, 30)]
    [InlineData(1000, PaymentMethod.DebitCard, 10)]
    [InlineData(1000, PaymentMethod.BankTransfer, 10)]
    [InlineData(100, PaymentMethod.BankTransfer, 10)]  // 最低手續費
    [InlineData(5000, PaymentMethod.BankTransfer, 25)] // 5000 * 0.005 = 25
    public void CalculateFee_不同付款方式_應計算正確手續費(
        decimal amount, PaymentMethod method, decimal expected)
    {
        // Arrange
        var processor = new PaymentProcessor();
        var type = typeof(PaymentProcessor);
        var methodInfo = type.GetMethod("CalculateFee", BindingFlags.NonPublic | BindingFlags.Instance);

        // Act
        var actual = (decimal)(methodInfo!.Invoke(processor, [amount, method]) ?? 0m);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("2024-03-15", true)]  // 星期五
    [InlineData("2024-03-16", false)] // 星期六
    [InlineData("2024-03-17", false)] // 星期日
    [InlineData("2024-03-18", true)]  // 星期一
    public void IsBusinessDay_不同日期_應回傳正確結果(string dateString, bool expected)
    {
        // Arrange
        var date = DateTime.Parse(dateString);
        var type = typeof(PaymentProcessor);
        var methodInfo = type.GetMethod("IsBusinessDay", BindingFlags.NonPublic | BindingFlags.Static);

        // Act
        var actual = (bool)(methodInfo!.Invoke(null, [date]) ?? false);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void CalculateFee_使用輔助方法_應計算正確手續費()
    {
        // Arrange
        var processor = new PaymentProcessor();
        var amount = 1000m;
        var method = PaymentMethod.CreditCard;

        // Act
        var actual = ReflectionTestHelper.InvokePrivateMethod<decimal>(
            processor, "CalculateFee", amount, method);

        // Assert
        actual.Should().Be(30m);
    }

    [Fact]
    public void IsBusinessDay_使用輔助方法_應正確判斷工作日()
    {
        // Arrange
        var date = new DateTime(2024, 3, 15); // 星期五

        // Act
        var actual = ReflectionTestHelper.InvokePrivateStaticMethod<bool>(
            typeof(PaymentProcessor), "IsBusinessDay", date);

        // Assert
        actual.Should().BeTrue();
    }
}