using System.Text;
using Day19.WebApplication.Integration.Tests.Infrastructure;
using Day19.WebApplication.Models;
using Xunit.Abstractions;

namespace Day19.WebApplication.Integration.Tests.Controllers;

/// <summary>
/// ShippersController 的整合測試
/// </summary>
public class ShippersControllerTests : IntegrationTestBase
{
    private readonly ITestOutputHelper _output;

    public ShippersControllerTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
        : base(factory)
    {
        this._output = output;
    }

    [Fact]
    public async Task GetShipper_當貨運商存在_應回傳成功結果()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var shipperId = await this.SeedShipperAsync("順豐速運", "02-2345-6789");

        // Act
        var response = await this.Client.GetAsync($"/api/shippers/{shipperId}");

        // Assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<ShipperOutputModel>>(result =>
                {
                    result.Status.Should().Be("Success");
                    result.Data.Should().NotBeNull();
                    result.Data!.ShipperId.Should().Be(shipperId);
                    result.Data.CompanyName.Should().Be("順豐速運");
                    result.Data.Phone.Should().Be("02-2345-6789");
                });
    }

    [Fact]
    public async Task GetShipper_當貨運商不存在_應回傳404()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var nonExistentId = 999;

        // Act
        var response = await this.Client.GetAsync($"/api/shippers/{nonExistentId}");

        // Assert
        response.Should().Be404NotFound();
    }

    [Fact]
    public async Task CreateShipper_輸入有效資料_應建立成功()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var createParameter = new ShipperCreateParameter
        {
            CompanyName = "黑貓宅急便",
            Phone = "02-1234-5678"
        };

        var jsonContent = JsonSerializer.Serialize(createParameter);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await this.Client.PostAsync("/api/shippers", content);

        // Assert
        response.Should().Be201Created()
                .And
                .Satisfy<SuccessResultOutputModel<ShipperOutputModel>>(result =>
                {
                    result.Status.Should().Be("Success");
                    result.Data.Should().NotBeNull();
                    result.Data!.ShipperId.Should().BeGreaterThan(0);
                    result.Data.CompanyName.Should().Be("黑貓宅急便");
                    result.Data.Phone.Should().Be("02-1234-5678");
                });
    }

    [Fact]
    public async Task UpdateShipper_當貨運商存在_應更新成功()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var shipperId = await this.SeedShipperAsync("舊公司名稱", "02-1111-1111");

        var updateParameter = new ShipperCreateParameter
        {
            CompanyName = "新公司名稱",
            Phone = "02-2222-2222"
        };

        var jsonContent = JsonSerializer.Serialize(updateParameter);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await this.Client.PutAsync($"/api/shippers/{shipperId}", content);

        // Assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<ShipperOutputModel>>(result =>
                {
                    result.Status.Should().Be("Success");
                    result.Data.Should().NotBeNull();
                    result.Data!.ShipperId.Should().Be(shipperId);
                    result.Data.CompanyName.Should().Be("新公司名稱");
                    result.Data.Phone.Should().Be("02-2222-2222");
                });
    }

    [Fact]
    public async Task UpdateShipper_當貨運商不存在_應回傳404()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var nonExistentId = 999;

        var updateParameter = new ShipperCreateParameter
        {
            CompanyName = "新公司名稱",
            Phone = "02-2222-2222"
        };

        var jsonContent = JsonSerializer.Serialize(updateParameter);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await this.Client.PutAsync($"/api/shippers/{nonExistentId}", content);

        // Assert
        response.Should().Be404NotFound();
    }

    [Fact]
    public async Task DeleteShipper_當貨運商存在_應刪除成功()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var shipperId = await this.SeedShipperAsync("待刪除公司", "02-3333-3333");

        // Act
        var response = await this.Client.DeleteAsync($"/api/shippers/{shipperId}");

        // Assert
        response.Should().Be204NoContent();
    }

    [Fact]
    public async Task DeleteShipper_當貨運商不存在_應回傳404()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var nonExistentId = 999;

        // Act
        var response = await this.Client.DeleteAsync($"/api/shippers/{nonExistentId}");

        // Assert
        response.Should().Be404NotFound();
    }

    [Fact]
    public async Task GetAllShippers_應回傳所有貨運商()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        await this.SeedShipperAsync("公司A", "02-1111-1111");
        await this.SeedShipperAsync("公司B", "02-2222-2222");

        // Act
        var response = await this.Client.GetAsync("/api/shippers");

        // Assert
        response.Should().Be200Ok()
                .And
                .Satisfy<SuccessResultOutputModel<List<ShipperOutputModel>>>(result =>
                {
                    result.Status.Should().Be("Success");
                    result.Data.Should().NotBeNull();
                    result.Data!.Count.Should().Be(2);
                    result.Data.Should().Contain(s => s.CompanyName == "公司A");
                    result.Data.Should().Contain(s => s.CompanyName == "公司B");
                });
    }

    #region 參數驗證測試

    [Fact]
    public async Task GetShipper_當ID為0_應回傳404()
    {
        // Arrange
        await this.CleanupDatabaseAsync();

        // Act
        var response = await this.Client.GetAsync("/api/shippers/0");

        // Assert
        response.Should().Be404NotFound();
    }

    [Fact]
    public async Task GetShipper_當ID為負數_應回傳404()
    {
        // Arrange
        await this.CleanupDatabaseAsync();

        // Act
        var response = await this.Client.GetAsync("/api/shippers/-1");

        // Assert
        response.Should().Be404NotFound();
    }

    [Fact]
    public async Task CreateShipper_當公司名稱為空_應回傳400BadRequest()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var createParameter = new ShipperCreateParameter
        {
            CompanyName = "", // 空字串
            Phone = "02-1234-5678"
        };

        var jsonContent = JsonSerializer.Serialize(createParameter);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await this.Client.PostAsync("/api/shippers", content);

        // Assert
        response.Should().Be400BadRequest()
                .And
                .Satisfy<ValidationProblemDetails>(problem =>
                {
                    problem.Type.Should().Be("https://tools.ietf.org/html/rfc9110#section-15.5.1");
                    problem.Title.Should().Be("One or more validation errors occurred.");
                    problem.Status.Should().Be(400);
                    problem.Errors.Should().ContainKey("CompanyName");
                    problem.Errors["CompanyName"].Should().Contain("公司名稱為必填");
                });
    }

    [Fact]
    public async Task CreateShipper_當公司名稱超過40字元_應回傳400BadRequest()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var createParameter = new ShipperCreateParameter
        {
            CompanyName = new string('A', 41), // 41個字元
            Phone = "02-1234-5678"
        };

        var jsonContent = JsonSerializer.Serialize(createParameter);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await this.Client.PostAsync("/api/shippers", content);

        // Assert
        response.Should().Be400BadRequest()
                .And.Satisfy<ValidationProblemDetails>(problem =>
                {
                    problem.Status.Should().Be(400);
                    problem.Errors.Should().ContainKey("CompanyName");
                    problem.Errors["CompanyName"].Should().Contain("公司名稱不可超過 40 個字元");
                });
    }

    [Fact]
    public async Task CreateShipper_當電話號碼超過24字元_應回傳400BadRequest()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var createParameter = new ShipperCreateParameter
        {
            CompanyName = "測試公司",
            Phone = new string('1', 25) // 25個字元
        };

        var jsonContent = JsonSerializer.Serialize(createParameter);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await this.Client.PostAsync("/api/shippers", content);

        // Assert
        response.Should().Be400BadRequest()
                .And.Satisfy<ValidationProblemDetails>(problem =>
                {
                    problem.Status.Should().Be(400);
                    problem.Errors.Should().ContainKey("Phone");
                    problem.Errors["Phone"].Should().Contain("電話號碼不可超過 24 個字元");
                });
    }

    [Fact]
    public async Task CreateShipper_當請求內容格式不正確_應回傳400BadRequest()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var invalidJson = "{ invalid json }";
        var content = new StringContent(invalidJson, Encoding.UTF8, "application/json");

        // Act
        var response = await this.Client.PostAsync("/api/shippers", content);

        // Assert
        response.Should().Be400BadRequest();
    }

    #endregion

    #region 邊界值測試

    [Fact]
    public async Task CreateShipper_當公司名稱剛好40字元_應建立成功()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var createParameter = new ShipperCreateParameter
        {
            CompanyName = new string('測', 20), // 20個中文字 = 40個字元
            Phone = "02-1234-5678"
        };

        var jsonContent = JsonSerializer.Serialize(createParameter);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await this.Client.PostAsync("/api/shippers", content);

        // Assert
        response.Should().Be201Created()
                .And.Satisfy<SuccessResultOutputModel<ShipperOutputModel>>(result =>
                {
                    result.Status.Should().Be("Success");
                    result.Data.Should().NotBeNull();
                    result.Data!.CompanyName.Should().Be(new string('測', 20));
                });
    }

    [Fact]
    public async Task CreateShipper_當電話號碼剛好24字元_應建立成功()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var createParameter = new ShipperCreateParameter
        {
            CompanyName = "測試公司",
            Phone = new string('1', 24) // 24個字元
        };

        var jsonContent = JsonSerializer.Serialize(createParameter);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var response = await this.Client.PostAsync("/api/shippers", content);

        // Assert
        response.Should().Be201Created()
                .And.Satisfy<SuccessResultOutputModel<ShipperOutputModel>>(result =>
                {
                    result.Status.Should().Be("Success");
                    result.Data.Should().NotBeNull();
                    result.Data!.Phone.Should().Be(new string('1', 24));
                });
    }

    [Fact]
    public async Task GetShipper_當ID為最大整數值_應回傳404()
    {
        // Arrange
        await this.CleanupDatabaseAsync();

        // Act
        var response = await this.Client.GetAsync($"/api/shippers/{int.MaxValue}");

        // Assert
        response.Should().Be404NotFound();
    }

    #endregion

    #region 業務邏輯測試

    [Fact]
    public async Task UpdateShipper_更新後驗證資料確實變更()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var shipperId = await this.SeedShipperAsync("原始公司", "02-1111-1111");

        var updateParameter = new ShipperCreateParameter
        {
            CompanyName = "更新後公司",
            Phone = "02-2222-2222"
        };

        var jsonContent = JsonSerializer.Serialize(updateParameter);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Act
        var updateResponse = await this.Client.PutAsync($"/api/shippers/{shipperId}", content);

        // Assert - 驗證更新成功
        updateResponse.Should().Be200Ok()
                      .And.Satisfy<SuccessResultOutputModel<ShipperOutputModel>>(result =>
                      {
                          result.Status.Should().Be("Success");
                          result.Data.Should().NotBeNull();
                          result.Data!.CompanyName.Should().Be("更新後公司");
                          result.Data.Phone.Should().Be("02-2222-2222");
                      });

        // 再次查詢驗證資料確實變更
        var getResponse = await this.Client.GetAsync($"/api/shippers/{shipperId}");
        getResponse.Should().Be200Ok()
                   .And.Satisfy<SuccessResultOutputModel<ShipperOutputModel>>(result =>
                   {
                       result.Data!.CompanyName.Should().Be("更新後公司");
                       result.Data.Phone.Should().Be("02-2222-2222");
                   });
    }

    [Fact]
    public async Task DeleteShipper_刪除後確認資料不存在()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var shipperId = await this.SeedShipperAsync("待刪除公司", "02-3333-3333");

        // Act - 執行刪除
        var deleteResponse = await this.Client.DeleteAsync($"/api/shippers/{shipperId}");

        // Assert - 驗證刪除成功
        deleteResponse.Should().Be204NoContent();

        // 再次查詢確認資料不存在
        var getResponse = await this.Client.GetAsync($"/api/shippers/{shipperId}");
        getResponse.Should().Be404NotFound();
    }

    [Fact]
    public async Task CreateMultipleShippers_驗證ID自動遞增()
    {
        // Arrange
        await this.CleanupDatabaseAsync();

        // Act - 建立多個貨運商
        var firstShipper = await this.CreateShipperViaApi("第一家公司", "02-1111-1111");
        var secondShipper = await this.CreateShipperViaApi("第二家公司", "02-2222-2222");
        var thirdShipper = await this.CreateShipperViaApi("第三家公司", "02-3333-3333");

        // Assert - 驗證ID遞增
        secondShipper.Should().Be(firstShipper + 1);
        thirdShipper.Should().Be(secondShipper + 1);
    }

    #endregion

    #region 輔助方法

    private async Task<int> CreateShipperViaApi(string companyName, string phone)
    {
        var createParameter = new ShipperCreateParameter
        {
            CompanyName = companyName,
            Phone = phone
        };

        var jsonContent = JsonSerializer.Serialize(createParameter);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await this.Client.PostAsync("/api/shippers", content);

        response.Should().Be201Created()
                .And.Satisfy<SuccessResultOutputModel<ShipperOutputModel>>(result =>
                {
                    result.Status.Should().Be("Success");
                    result.Data.Should().NotBeNull();
                    result.Data!.CompanyName.Should().Be(companyName);
                    result.Data.Phone.Should().Be(phone);
                });

        // 取得建立的 ShipperId
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<SuccessResultOutputModel<ShipperOutputModel>>(
            responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result!.Data!.ShipperId;
    }

    #endregion
}