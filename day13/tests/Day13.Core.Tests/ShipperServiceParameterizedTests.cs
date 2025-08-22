using System;

namespace Day13.Core.Tests;

/// <summary>
/// ShipperService 參數化測試
/// </summary>
public class ShipperServiceParameterizedTests
{
    /// <summary>
    /// 測試 GetCollectionAsync 的參數驗證
    /// </summary>
    [Theory]
    [InlineAutoDataWithCustomization(0, 10, nameof(from))]
    [InlineAutoDataWithCustomization(-1, 10, nameof(from))]
    [InlineAutoDataWithCustomization(1, 0, nameof(size))]
    [InlineAutoDataWithCustomization(1, -1, nameof(size))]
    public async Task GetCollectionAsync_from與size輸入不合規格內容_應拋出ArgumentOutOfRangeException(
        int from, int size, string parameterName, ShipperService sut)
    {
        // Act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => sut.GetCollectionAsync(from, size));

        // Assert
        exception.Message.Should().Contain(parameterName);
    }

    /// <summary>
    /// 測試 GetAsync 的參數驗證
    /// </summary>
    [Theory]
    [InlineAutoDataWithCustomization(0)]
    [InlineAutoDataWithCustomization(-1)]
    [InlineAutoDataWithCustomization(-100)]
    public async Task GetAsync_輸入無效的ShipperId_應拋出ArgumentOutOfRangeException(
        int invalidShipperId, ShipperService sut)
    {
        // Act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => sut.GetAsync(invalidShipperId));

        // Assert
        exception.ParamName.Should().Be("shipperId");
    }

    /// <summary>
    /// 測試 SearchAsync 的不同搜尋條件
    /// </summary>
    [Theory]
    [InlineAutoDataWithCustomization("", "")]
    [InlineAutoDataWithCustomization(null!, null!)]
    [InlineAutoDataWithCustomization("   ", "   ")]
    public async Task SearchAsync_輸入空白或null的搜尋條件_應拋出ArgumentException(
        string? companyName, string? phone, ShipperService sut)
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => sut.SearchAsync(companyName!, phone!));

        exception.Message.Should().Contain("companyName 與 phone 不可都為空白");
    }
}