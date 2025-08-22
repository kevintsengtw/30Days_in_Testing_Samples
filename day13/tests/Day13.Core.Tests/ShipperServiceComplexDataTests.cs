using System;

namespace Day13.Core.Tests;

/// <summary>
/// ShipperService 複雜資料設定測試
/// </summary>
public class ShipperServiceComplexDataTests
{
    /// <summary>
    /// 測試複雜的資料設定 - 使用 IFixture 參數
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public async Task SearchAsync_companyName輸入資料_phone無輸入_有符合條件的資料_回傳集合應包含符合條件的資料(
        IFixture fixture,
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // Arrange
        var models = fixture.Build<ShipperModel>()
                            .With(x => x.CompanyName, "test")
                            .CreateMany(1);

        shipperRepository.GetTotalCountAsync().Returns(1);
        shipperRepository.SearchAsync(Arg.Any<string>(), Arg.Any<string>())
                         .Returns(models);

        const string companyName = "test";
        const string phone = "";

        // Act
        var actual = await sut.SearchAsync(companyName, phone);

        // Assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(1);
        actual.Any(x => x.CompanyName == companyName).Should().BeTrue();
    }

    /// <summary>
    /// 測試複雜的資料設定 - 建立多筆特定資料
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public async Task SearchAsync_建立多筆特定資料_驗證對應正確性(
        IFixture fixture,
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // Arrange
        var models = fixture.Build<ShipperModel>()
                            .With(x => x.CompanyName, "Microsoft")
                            .With(x => x.Country, "USA")
                            .CreateMany(3);

        shipperRepository.GetTotalCountAsync().Returns(3);
        shipperRepository.SearchAsync(Arg.Any<string>(), Arg.Any<string>())
                         .Returns(models);

        // Act
        var actual = await sut.SearchAsync("Microsoft", "");

        // Assert
        actual.Should().HaveCount(3);
        actual.All(x => x.CompanyName == "Microsoft").Should().BeTrue();
        actual.All(x => x.Country == "USA").Should().BeTrue();
    }

    /// <summary>
    /// 測試 GetCollectionAsync 當總數為 0 時的行為
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public async Task GetCollectionAsync_當資料總數為0_應回傳空集合(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // Arrange
        shipperRepository.GetTotalCountAsync().Returns(0);

        // Act
        var actual = await sut.GetCollectionAsync(1, 10);

        // Assert
        actual.Should().BeEmpty();

        // 驗證不會呼叫 GetCollectionAsync，因為總數為 0
        await shipperRepository.DidNotReceive().GetCollectionAsync(Arg.Any<int>(), Arg.Any<int>());
    }

    /// <summary>
    /// 測試 GetCollectionAsync 正常情況
    /// </summary>
    [Theory]
    [AutoDataWithCustomization]
    public async Task GetCollectionAsync_正常分頁請求_應回傳對應資料(
        IFixture fixture,
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // Arrange
        const int totalCount = 100;
        const int pageSize = 10;
        const int from = 1;

        var models = fixture.CreateMany<ShipperModel>(pageSize);

        shipperRepository.GetTotalCountAsync().Returns(totalCount);
        shipperRepository.GetCollectionAsync(from, pageSize).Returns(models);

        // Act
        var actual = await sut.GetCollectionAsync(from, pageSize);

        // Assert
        actual.Should().HaveCount(pageSize);

        // 驗證有呼叫對應的方法
        await shipperRepository.Received(1).GetTotalCountAsync();
        await shipperRepository.Received(1).GetCollectionAsync(from, pageSize);
    }
}