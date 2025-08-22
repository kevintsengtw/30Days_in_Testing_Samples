using System;
using Day13.Core.Misc;

namespace Day13.Core.Tests;

/// <summary>
/// 完整的 ShipperService 單元測試
/// </summary>
public class ShipperServiceTests
{
    //---------------------------------------------------------------------------------------------
    // IsExistsAsync

    [Theory]
    [AutoDataWithCustomization]
    public async Task IsExistsAsync_輸入的ShipperId為0時_應拋出ArgumentOutOfRangeException(ShipperService sut)
    {
        // arrange
        var shipperId = 0;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => sut.IsExistsAsync(shipperId));

        // assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task IsExistsAsync_輸入的ShipperId為負1時_應拋出ArgumentOutOfRangeException(ShipperService sut)
    {
        // arrange
        const int shipperId = -1;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => sut.IsExistsAsync(shipperId));

        // assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task IsExistsAsync_輸入的ShipperId_資料不存在_應回傳false(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // arrange
        const int shipperId = 99;

        shipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(false);

        // act
        var actual = await sut.IsExistsAsync(shipperId);

        // assert
        actual.Should().BeFalse();
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task IsExistsAsync_輸入的ShipperId_資料有存在_應回傳True(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // arrange
        const int shipperId = 99;

        shipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(true);

        // act
        var actual = await sut.IsExistsAsync(shipperId);

        // assert
        actual.Should().BeTrue();
    }

    //---------------------------------------------------------------------------------------------
    // GetAsync

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAsync_輸入的ShipperId為0時_應拋出ArgumentOutOfRangeException(ShipperService sut)
    {
        // arrange
        const int shipperId = 0;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => sut.GetAsync(shipperId));

        // assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAsync_輸入的ShipperId為負1時_應拋出ArgumentOutOfRangeException(ShipperService sut)
    {
        // arrange
        const int shipperId = -1;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => sut.GetAsync(shipperId));

        // assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAsync_輸入的ShipperId_資料不存在_應回傳null(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // arrange
        const int shipperId = 99;

        shipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(false);

        // act
        var actual = await sut.GetAsync(shipperId);

        // assert
        actual.Should().BeNull();
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAsync_輸入的ShipperId_資料有存在_應回傳model(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut,
        ShipperModel model)
    {
        // arrange
        var shipperId = model.ShipperId;

        shipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(true);
        shipperRepository.GetAsync(Arg.Any<int>()).Returns(model);

        // act
        var actual = await sut.GetAsync(shipperId);

        // assert
        actual.Should().NotBeNull();
        actual.ShipperId.Should().Be(shipperId);
    }

    //---------------------------------------------------------------------------------------------
    // GetTotalCountAsync

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetTotalCountAsync_資料表裡無資料_應回傳0(ShipperService sut)
    {
        // arrange
        const int expected = 0;

        // act
        var actual = await sut.GetTotalCountAsync();

        // assert
        actual.Should().Be(expected);
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetTotalCountAsync_資料表裡有10筆資料_應回傳10(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // arrange
        const int expected = 10;

        shipperRepository.GetTotalCountAsync().Returns(10);

        // act
        var actual = await sut.GetTotalCountAsync();

        // assert
        actual.Should().Be(expected);
    }

    //---------------------------------------------------------------------------------------------
    // GetAllAsync

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAllAsync_資料表裡無資料_應回傳空集合(ShipperService sut)
    {
        // arrange

        // act
        var actual = await sut.GetAllAsync();

        // assert
        actual.Should().BeEmpty();
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetAllAsync_資料表裡有10筆資料_回傳的集合裡有10筆(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut,
        [CollectionSize(10)] IEnumerable<ShipperModel> models)
    {
        // arrange
        shipperRepository.GetAllAsync().Returns(models);

        // act
        var actual = await sut.GetAllAsync();

        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(10);
    }

    //---------------------------------------------------------------------------------------------
    // GetCollectionAsync

    [Theory]
    [InlineAutoDataWithCustomization(0, 10, nameof(from))]
    [InlineAutoDataWithCustomization(-1, 10, nameof(from))]
    [InlineAutoDataWithCustomization(1, 0, nameof(size))]
    [InlineAutoDataWithCustomization(1, -1, nameof(size))]
    public async Task GetCollectionAsync_from與size輸入不合規格內容_應拋出ArgumentOutOfRangeException(
        int from, int size, string parameterName, ShipperService sut)
    {
        // act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => sut.GetCollectionAsync(from, size));

        // assert
        exception.Message.Should().Contain(parameterName);
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetCollectionAsync_from為1_size為10_資料表裡無資料_應回傳空集合(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // arrange
        const int from = 1;
        const int size = 10;

        shipperRepository.GetTotalCountAsync().Returns(0);

        // act
        var actual = await sut.GetCollectionAsync(from, size);

        // assert
        actual.Should().BeEmpty();
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetCollectionAsync_from為20_size為10_資料表裡只有10筆資料_from超過總數量_應回傳空集合(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // arrange
        const int from = 20;
        const int size = 10;

        shipperRepository.GetTotalCountAsync().Returns(10);

        // act
        var actual = await sut.GetCollectionAsync(from, size);

        // assert
        actual.Should().BeEmpty();
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetCollectionAsync_from為1_size為10_資料表裡有5筆資料_回傳集合應有5筆(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut,
        [CollectionSize(5)] IEnumerable<ShipperModel> models)
    {
        // arrange
        const int from = 1;
        const int size = 10;

        shipperRepository.GetTotalCountAsync().Returns(5);

        shipperRepository
            .GetCollectionAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(models);

        // act
        var actual = await sut.GetCollectionAsync(from, size);

        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(5);
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetCollectionAsync_from為6_size為10_資料表裡有10筆資料_回傳集合應有10筆(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut,
        [CollectionSize(20)] IEnumerable<ShipperModel> models)
    {
        // arrange
        const int from = 6;
        const int size = 10;

        shipperRepository.GetTotalCountAsync().Returns(10);

        shipperRepository
            .GetCollectionAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(models.Skip(5).Take(10));

        // act
        var actual = await sut.GetCollectionAsync(from, size);

        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(10);
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task GetCollectionAsync_from為11_size為10_資料表裡有30筆資料_回傳集合應有10筆(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut,
        [CollectionSize(30)] IEnumerable<ShipperModel> models)
    {
        // arrange
        const int from = 11;
        const int size = 10;

        shipperRepository.GetTotalCountAsync().Returns(30);

        shipperRepository
            .GetCollectionAsync(Arg.Any<int>(), Arg.Any<int>())
            .Returns(models.Skip(10).Take(10));

        // act
        var actual = await sut.GetCollectionAsync(from, size);

        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(10);
    }

    //---------------------------------------------------------------------------------------------
    // SearchAsync

    [Theory]
    [InlineAutoDataWithCustomization(null!, null!)]
    [InlineAutoDataWithCustomization("", null!)]
    [InlineAutoDataWithCustomization(null!, "")]
    [InlineAutoDataWithCustomization(null!, null!)]
    public async Task SearchAsync_companyName與phone輸入不合規格的內容_應拋出ArgumentException(
        string? companyName, string? phone, ShipperService sut)
    {
        // arrange
        const string exceptionMessage = "companyName 與 phone 不可都為空白";

        // act
        var exception = await Assert.ThrowsAsync<ArgumentException>(
            () => sut.SearchAsync(companyName!, phone!));

        // assert
        exception.Message.Should().Be(exceptionMessage);
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task SearchAsync_資料表裡無資料_應回傳空集合(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // arrange
        const string companyName = "test";
        const string phone = "02123456789";

        shipperRepository.GetTotalCountAsync().Returns(0);

        // act
        var actual = await sut.SearchAsync(companyName, phone);

        // assert
        actual.Should().BeEmpty();
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task SearchAsync_companyName輸入資料_沒有符合條件的資料_應回傳空集合(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // arrange
        const string companyName = "test";
        const string phone = "";

        shipperRepository.GetTotalCountAsync().Returns(10);

        shipperRepository
            .SearchAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns([]);

        // act
        var actual = await sut.SearchAsync(companyName, phone);

        // assert
        actual.Should().BeEmpty();
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task SearchAsync_companyName輸入資料_phone無輸入_有符合條件的資料_回傳集合應包含符合條件的資料(
        IFixture fixture,
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // arrange
        var models = fixture.Build<ShipperModel>()
                            .With(x => x.CompanyName, "test")
                            .CreateMany(1);

        shipperRepository.GetTotalCountAsync().Returns(10);

        shipperRepository.SearchAsync(Arg.Any<string>(), Arg.Any<string>())
                         .Returns(models);

        const string companyName = "test";
        const string phone = "";

        // act
        var actual = await sut.SearchAsync(companyName, phone);

        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(1);
        actual.Any(x => x.CompanyName == companyName).Should().BeTrue();
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task SearchAsync_companyName無輸入_phone輸入資料_有符合條件的資料_回傳集合應包含符合條件的資料(
        IFixture fixture,
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // arrange
        var models = fixture.Build<ShipperModel>()
                            .With(x => x.Phone, "02123456789")
                            .CreateMany(1);

        shipperRepository.GetTotalCountAsync().Returns(10);

        shipperRepository.SearchAsync(Arg.Any<string>(), Arg.Any<string>())
                         .Returns(models);

        const string companyName = "";
        const string phone = "02123456789";

        // act
        var actual = await sut.SearchAsync(companyName, phone);

        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(1);
        actual.Any(x => x.Phone == phone).Should().BeTrue();
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task SearchAsync_companyName輸入資料_phone輸入資料_有符合條件的資料_回傳集合應包含符合條件的資料(
        IFixture fixture,
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // arrange
        var models = fixture.Build<ShipperModel>()
                            .With(x => x.CompanyName, "demo")
                            .With(x => x.Phone, "03123456789")
                            .CreateMany(1);

        shipperRepository.GetTotalCountAsync().Returns(11);

        shipperRepository.SearchAsync(Arg.Any<string>(), Arg.Any<string>())
                         .Returns(models);

        const string companyName = "demo";
        const string phone = "03123456789";

        // act
        var actual = await sut.SearchAsync(companyName, phone);

        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(1);
        actual.Any(x => x is { CompanyName: companyName, Phone: phone }).Should().BeTrue();
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task SearchAsync_companyName輸入資料_phone輸入資料_沒有符合條件的資料_應回傳空集合(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // arrange
        shipperRepository.GetTotalCountAsync().Returns(10);

        shipperRepository.SearchAsync(Arg.Any<string>(), Arg.Any<string>())
                         .Returns([]);

        const string companyName = "try";
        const string phone = "04123456789";

        // act
        var actual = await sut.SearchAsync(companyName, phone);

        // assert
        actual.Should().BeEmpty();
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task SearchAsync_companyName輸入資料_phone無輸入_有2筆符合條件的資料_回傳集合應有兩筆(
        IFixture fixture,
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // arrange
        var model1 = fixture.Build<ShipperModel>()
                            .With(x => x.CompanyName, "note")
                            .Create();

        var model2 = fixture.Build<ShipperModel>()
                            .With(x => x.CompanyName, "node")
                            .Create();

        var models = new List<ShipperModel>
        {
            model1,
            model2
        };

        shipperRepository.GetTotalCountAsync().Returns(10);

        shipperRepository.SearchAsync(Arg.Any<string>(), Arg.Any<string>())
                         .Returns(models);

        const string companyName = "no";
        const string phone = "";

        // act
        var actual = await sut.SearchAsync(companyName, phone);

        // assert
        actual.Should().NotBeEmpty();
        actual.Should().HaveCount(2);
        actual.All(x => x.CompanyName.StartsWith("no")).Should().BeTrue();
    }

    //---------------------------------------------------------------------------------------------
    // CreateAsync

    [Theory]
    [AutoDataWithCustomization]
    public async Task CreateAsync_輸入的model為null時_應拋出ArgumentNullException(ShipperService sut)
    {
        // arrange
        ShipperDto? shipperDto = null;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => sut.CreateAsync(shipperDto!));

        // assert
        exception.Message.Should().Contain("shipper");
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task CreateAsync_輸入一個有資料的model_新增完成_回傳Result的Success應為true(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut,
        ShipperDto shipperDto)
    {
        // arrange
        shipperRepository.CreateAsync(Arg.Any<ShipperModel>())
                         .Returns(new Result { Success = true, AffectRows = 1 });

        // act
        var actual = await sut.CreateAsync(shipperDto);

        // assert
        actual.Success.Should().BeTrue();
        actual.AffectRows.Should().Be(1);
    }

    //---------------------------------------------------------------------------------------------
    // UpdateAsync

    [Theory]
    [AutoDataWithCustomization]
    public async Task UpdateAsync_輸入的model為null時_應拋出ArgumentNullException(ShipperService sut)
    {
        // arrange
        ShipperDto? shipperDto = null;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => sut.UpdateAsync(shipperDto!));

        // assert
        exception.Message.Should().Contain("shipper");
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task UpdateAsync_輸入model_要修改的資料並不存在_更新錯誤_回傳Result的Success應為false(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut,
        ShipperDto shipperDto)
    {
        // arrange
        shipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(false);

        // act
        var actual = await sut.UpdateAsync(shipperDto);

        // assert
        actual.Success.Should().BeFalse();
        actual.Message.Should().Be("shipper not exists");
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task UpdateAsync_輸入model_要修改的資料存在_更新完成_回傳Result的Success應為true(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut,
        ShipperDto shipperDto)
    {
        // arrange
        shipperDto.CompanyName = "update";

        shipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(true);

        shipperRepository.UpdateAsync(Arg.Any<ShipperModel>())
                         .Returns(new Result { Success = true, AffectRows = 1 });

        // act
        var actual = await sut.UpdateAsync(shipperDto);

        // assert
        actual.Success.Should().BeTrue();
    }

    //---------------------------------------------------------------------------------------------
    // DeleteAsync

    [Theory]
    [AutoDataWithCustomization]
    public async Task DeleteAsync_輸入的ShipperId為0時_應拋出ArgumentOutOfRangeException(ShipperService sut)
    {
        // arrange
        const int shipperId = 0;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => sut.DeleteAsync(shipperId));

        // assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task DeleteAsync_輸入的ShipperId為負1時_應拋出ArgumentOutOfRangeException(ShipperService sut)
    {
        // arrange
        const int shipperId = -1;

        // act
        var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => sut.DeleteAsync(shipperId));

        // assert
        exception.Message.Should().Contain(nameof(shipperId));
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task DeleteAsync_輸入ShipperId_要刪除的資料並不存在_刪除錯誤_回傳Result的Success應為false(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut)
    {
        // arrange
        const int shipperId = 999;

        shipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(false);

        // act
        var actual = await sut.DeleteAsync(shipperId);

        // assert
        actual.Success.Should().BeFalse();
        actual.Message.Should().Be("shipper not exists");
    }

    [Theory]
    [AutoDataWithCustomization]
    public async Task DeleteAsync_輸入model_要刪除的資料存在_刪除完成_回傳Result的Success應為true(
        [Frozen] IShipperRepository shipperRepository,
        ShipperService sut,
        ShipperDto shipperDto)
    {
        // arrange
        shipperRepository.IsExistsAsync(Arg.Any<int>()).Returns(true);

        shipperRepository.DeleteAsync(Arg.Any<int>())
                         .Returns(new Result { Success = true, AffectRows = 1 });

        // act
        var actual = await sut.DeleteAsync(shipperDto.ShipperId);

        // assert
        actual.Success.Should().BeTrue();
    }
}