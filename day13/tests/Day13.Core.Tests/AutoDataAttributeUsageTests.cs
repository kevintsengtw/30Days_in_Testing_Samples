using System;

namespace Day13.Core.Tests;

/// <summary>
/// 展示不同 AutoData 屬性的使用方式
/// </summary>
public class AutoDataAttributeUsageTests
{
    /// <summary>
    /// 使用標準 AutoData 屬性（沒有客製化）
    /// </summary>
    [Theory]
    [AutoData]
    public void StandardAutoData_不含客製化設定(
        ShipperModel model,
        ShipperDto dto)
    {
        // 這個測試使用標準的 AutoData，沒有我們的客製化設定
        // 所以 IMapper 會是 AutoFixture 預設的行為

        // Assert
        model.Should().NotBeNull();
        dto.Should().NotBeNull();

        model.ShipperId.Should().BeGreaterThan(0);
        dto.ShipperId.Should().BeGreaterThan(0);
    }

    /// <summary>
    /// 使用我們客製化的 AutoData 屬性
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public void CustomizedAutoData_包含客製化設定(
        ShipperService sut,
        ShipperModel model)
    {
        // 這個測試使用我們的客製化 AutoData
        // ShipperService 會自動注入 AutoNSubstitute 建立的 IShipperRepository
        // 以及真實的 Mapster IMapper 實作

        // Assert
        sut.Should().NotBeNull();
        model.Should().NotBeNull();

        // 可以驗證 AutoNSubstitute 有正確建立相依性
        model.ShipperId.Should().BeGreaterThan(0);
        model.CompanyName.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// 使用 InlineAutoData 混合固定值與自動產生
    /// </summary>
    [Theory]
    [InlineAutoDataWithCustomization("Microsoft")]
    [InlineAutoDataWithCustomization("Google")]
    [InlineAutoDataWithCustomization("Apple")]
    public async Task InlineAutoData_混合固定值與自動產生(
        string expectedCompanyName,
        IFixture fixture,
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // Arrange
        var models = fixture.Build<ShipperModel>()
                            .With(x => x.CompanyName, expectedCompanyName)
                            .CreateMany(1);

        shipperRepository.GetTotalCountAsync().Returns(1);
        shipperRepository.SearchAsync(Arg.Any<string>(), Arg.Any<string>())
                         .Returns(models);

        // Act
        var actual = await sut.SearchAsync(expectedCompanyName, "");

        // Assert
        actual.Should().HaveCount(1);
        actual.First().CompanyName.Should().Be(expectedCompanyName);
    }

    /// <summary>
    /// 使用 Frozen 屬性控制相依性
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public async Task FrozenAttribute_控制相依性實例(
        [Frozen] IShipperRepository repository1,
        [Frozen] IShipperRepository repository2,
        ShipperService sut)
    {
        // Arrange
        // repository1 和 repository2 應該是同一個實例
        // 因為都被 [Frozen] 標記

        repository1.IsExistsAsync(Arg.Any<int>()).Returns(true);

        // Act
        var result = await sut.IsExistsAsync(1);

        // Assert
        result.Should().BeTrue();

        // 驗證 repository1 和 repository2 是同一個實例
        repository1.Should().BeSameAs(repository2);

        // 驗證方法有被呼叫
        await repository1.Received(1).IsExistsAsync(1);
        // repository2 也會收到呼叫，因為它們是同一個實例
        await repository2.Received(1).IsExistsAsync(1);
    }

    /// <summary>
    /// 展示 CollectionSize 屬性的不同大小
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public void CollectionSize_不同大小的集合(
        [CollectionSize(1)] IEnumerable<ShipperModel> singleItem,
        [CollectionSize(5)] IEnumerable<ShipperModel> fiveItems,
        [CollectionSize(10)] IEnumerable<ShipperModel> tenItems)
    {
        // Assert
        singleItem.Should().HaveCount(1);
        fiveItems.Should().HaveCount(5);
        tenItems.Should().HaveCount(10);

        // 驗證每個集合中的物件都是有效的
        singleItem.All(x => x.ShipperId > 0).Should().BeTrue();
        fiveItems.All(x => x.ShipperId > 0).Should().BeTrue();
        tenItems.All(x => x.ShipperId > 0).Should().BeTrue();
    }
}