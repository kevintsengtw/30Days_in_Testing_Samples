using System;

namespace Day13.Core.Tests;

/// <summary>
/// ShipperService 基本測試
/// </summary>
public class ShipperServiceBasicTests
{
    /// <summary>
    /// 測試當 ShipperId 為 0 時應拋出 ArgumentOutOfRangeException
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public async Task IsExistsAsync_輸入的ShipperId為0時_應拋出ArgumentOutOfRangeException(ShipperService sut)
    {
        // Arrange
        const int shipperId = 0;

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.IsExistsAsync(shipperId));

        // Assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    /// <summary>
    /// 測試當 ShipperId 為負數時應拋出 ArgumentOutOfRangeException
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public async Task IsExistsAsync_輸入的ShipperId為負數時_應拋出ArgumentOutOfRangeException(ShipperService sut)
    {
        // Arrange
        const int shipperId = -1;

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.IsExistsAsync(shipperId));

        // Assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    /// <summary>
    /// 測試當資料不存在時應回傳 false
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public async Task IsExistsAsync_輸入的ShipperId_資料不存在_應回傳false(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // Arrange
        const int shipperId = 99;

        shipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(false);

        // Act
        var actual = await sut.IsExistsAsync(shipperId);

        // Assert
        actual.Should().BeFalse();
    }

    /// <summary>
    /// 測試當資料存在時應回傳 true
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public async Task IsExistsAsync_輸入的ShipperId_資料存在_應回傳true(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // Arrange
        const int shipperId = 1;

        shipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(true);

        // Act
        var actual = await sut.IsExistsAsync(shipperId);

        // Assert
        actual.Should().BeTrue();
    }
}