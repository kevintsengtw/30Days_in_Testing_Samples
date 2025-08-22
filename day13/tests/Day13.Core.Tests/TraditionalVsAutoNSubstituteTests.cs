using System;
using MapsterMapper;

namespace Day13.Core.Tests;

/// <summary>
/// 展示傳統方式與 AutoNSubstitute 方式的差異
/// </summary>
public class TraditionalVsAutoNSubstituteTests
{
    /// <summary>
    /// 傳統手動方式的測試範例
    /// </summary>
    [Fact]
    public async Task TraditionalWay_手動建立所有相依性()
    {
        // Arrange - 手動建立每個相依性
        var mapper = Substitute.For<IMapper>();
        var shipperRepository = Substitute.For<IShipperRepository>();
        var sut = new ShipperService(mapper, shipperRepository);

        // 手動設定 Mock 行為
        shipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(true);

        var shipperModel = new ShipperModel
        {
            ShipperId = 1,
            CompanyName = "Test Company",
            ContactName = "John Doe",
            Phone = "123-456-7890"
        };

        shipperRepository.GetAsync(Arg.Any<int>()).Returns(shipperModel);

        var shipperDto = new ShipperDto
        {
            ShipperId = 1,
            CompanyName = "Test Company",
            ContactName = "John Doe",
            Phone = "123-456-7890"
        };

        mapper.Map<ShipperModel, ShipperDto>(Arg.Any<ShipperModel>()).Returns(shipperDto);

        // Act
        var actual = await sut.GetAsync(1);

        // Assert
        actual.Should().NotBeNull();
        actual.ShipperId.Should().Be(1);
        actual.CompanyName.Should().Be("Test Company");
    }

    /// <summary>
    /// 使用 AutoNSubstitute 的簡化方式
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public async Task WithAutoNSubstitute_自動建立相依性並使用真實Mapper(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut,
        ShipperModel shipperModel)
    {
        // Arrange - 相依性已自動建立，只需設定需要的行為
        shipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(true);
        shipperRepository.GetAsync(Arg.Any<int>()).Returns(shipperModel);

        // Act
        var actual = await sut.GetAsync(shipperModel.ShipperId);

        // Assert
        actual.Should().NotBeNull();
        actual.ShipperId.Should().Be(shipperModel.ShipperId);
        actual.CompanyName.Should().Be(shipperModel.CompanyName);
        actual.ContactName.Should().Be(shipperModel.ContactName);

        // 注意：這裡的 IMapper 是真實的 Mapster 實作，不是 Mock
        // 所以可以真正驗證對應邏輯是否正確
    }

    /// <summary>
    /// 展示複雜相依性的差異
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public async Task ComplexDependencies_AutoNSubstitute方式處理多個相依性(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut,
        [CollectionSize(5)] IEnumerable<ShipperModel> models)
    {
        // Arrange
        // 即使 ShipperService 未來增加更多相依性，這個測試也不需要修改
        // AutoFixture 會自動處理新增的相依性
        shipperRepository.GetAllAsync().Returns(models);

        // Act
        var actual = await sut.GetAllAsync();

        // Assert
        actual.Should().HaveCount(5);

        // 驗證每個物件都正確對應
        foreach (var dto in actual)
        {
            dto.Should().NotBeNull();
            dto.ShipperId.Should().BeGreaterThan(0);
            dto.CompanyName.Should().NotBeNullOrEmpty();
        }
    }
}