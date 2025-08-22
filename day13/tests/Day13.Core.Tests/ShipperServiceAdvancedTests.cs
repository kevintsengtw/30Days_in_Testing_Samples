using System;

namespace Day13.Core.Tests;

/// <summary>
/// ShipperService 進階測試
/// </summary>
public class ShipperServiceAdvancedTests
{
    /// <summary>
    /// 測試使用自動產生的資料
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAsync_輸入的ShipperId_資料有存在_應回傳model(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut,
        ShipperModel model)
    {
        // Arrange
        var shipperId = model.ShipperId;

        shipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(true);
        shipperRepository.GetAsync(Arg.Any<int>()).Returns(model);

        // Act
        var actual = await sut.GetAsync(shipperId);

        // Assert
        actual.Should().NotBeNull();
        actual.ShipperId.Should().Be(shipperId);
        actual.CompanyName.Should().Be(model.CompanyName);
        actual.ContactName.Should().Be(model.ContactName);
    }

    /// <summary>
    /// 測試當資料不存在時應回傳 null
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAsync_輸入的ShipperId_資料不存在_應回傳null(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // Arrange
        var shipperId = 999;

        shipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(false);

        // Act
        var actual = await sut.GetAsync(shipperId);

        // Assert
        actual.Should().BeNull();
    }

    /// <summary>
    /// 測試使用 CollectionSize 控制集合大小
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAllAsync_資料表裡有10筆資料_回傳的集合裡有10筆(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut,
        [CollectionSize(10)] IEnumerable<ShipperModel> models)
    {
        // Arrange
        shipperRepository.GetAllAsync().Returns(models);

        // Act
        var actual = await sut.GetAllAsync();

        // Assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(10);
    }

    /// <summary>
    /// 測試使用 CollectionSize 控制集合大小 - 5筆資料
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAllAsync_資料表裡有5筆資料_回傳的集合裡有5筆(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut,
        [CollectionSize(5)] IEnumerable<ShipperModel> models)
    {
        // Arrange
        shipperRepository.GetAllAsync().Returns(models);

        // Act
        var actual = await sut.GetAllAsync();

        // Assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(5);
    }
}