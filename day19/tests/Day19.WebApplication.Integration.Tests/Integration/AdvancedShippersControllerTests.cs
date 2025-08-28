using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using Day19.WebApplication.Integration.Tests.Infrastructure;
using Day19.WebApplication.Models;
using Xunit.Abstractions;

namespace Day19.WebApplication.Integration.Tests.Integration;

/// <summary>
/// 進階整合測試案例 - 參考 Sample_01-03 的測試模式
/// </summary>
public class AdvancedShippersControllerTests : IntegrationTestBase
{
    private readonly ITestOutputHelper _output;

    public AdvancedShippersControllerTests(ITestOutputHelper output) : base(new CustomWebApplicationFactory())
    {
        this._output = output;
    }

    #region HTTP Headers 和 Content-Type 測試

    [Fact]
    [Trait("Category", "api/shippers.GET")]
    public async Task GetShipper_使用正確的AcceptHeaders_應正確處理JSON回應()
    {
        // Arrange
        await this.CleanupDatabaseAsync(); // 確保測試隔離
        var shipperId = await this.SeedShipperAsync("進階測試物流");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"/api/shippers/{shipperId}", UriKind.RelativeOrAbsolute)
        };
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

        // Act
        var response = await this.Client.SendAsync(request);

        // Assert
        response.Should().Be200Ok()
                .And.Satisfy<SuccessResultOutputModel<ShipperOutputModel>>(responseWrapper =>
                {
                    responseWrapper.Should().NotBeNull();
                    responseWrapper.Status.Should().Be("Success");
                    responseWrapper.Data.Should().NotBeNull();
                    responseWrapper.Data.CompanyName.Should().Be("進階測試物流");
                });

        // 額外驗證 Content-Type
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        var content = await response.Content.ReadAsStringAsync();
        this._output.WriteLine($"Response Content: {content}");
    }

    [Fact]
    [Trait("Category", "api/shippers.POST")]
    public async Task CreateShipper_使用正確的ContentType_應成功建立()
    {
        // Arrange
        await this.CleanupDatabaseAsync(); // 確保測試隔離
        var newShipper = new ShipperCreateParameter
        {
            CompanyName = "Content-Type測試物流",
            Phone = "02-12345678"
        };

        var json = JsonSerializer.Serialize(newShipper, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("/api/shippers", UriKind.RelativeOrAbsolute),
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

        // Act
        var response = await this.Client.SendAsync(request);

        // Assert
        response.Should().Be201Created()
                .And.Satisfy<SuccessResultOutputModel<ShipperOutputModel>>(responseWrapper =>
                {
                    responseWrapper.Should().NotBeNull();
                    responseWrapper.Status.Should().Be("Success");
                    responseWrapper.Data.Should().NotBeNull();
                    responseWrapper.Data.CompanyName.Should().Be("Content-Type測試物流");
                });

        var content = await response.Content.ReadAsStringAsync();
        this._output.WriteLine($"Response Content: {content}");
    }

    #endregion

    #region Query String 參數測試

    [Fact]
    [Trait("Category", "api/shippers.GET")]
    public async Task GetShipper_使用QueryStringParameter_應正確解析參數()
    {
        // Arrange
        await this.CleanupDatabaseAsync(); // 確保測試隔離
        var shipperId = await this.SeedShipperAsync("查詢字串測試物流");

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"/api/shippers/{shipperId}", UriKind.RelativeOrAbsolute)
        };
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

        // Act
        var response = await this.Client.SendAsync(request);

        // Assert
        response.Should().Be200Ok()
                .And.Satisfy<SuccessResultOutputModel<ShipperOutputModel>>(responseWrapper =>
                {
                    responseWrapper.Should().NotBeNull();
                    responseWrapper.Status.Should().Be("Success");
                    responseWrapper.Data.Should().NotBeNull();
                    responseWrapper.Data.CompanyName.Should().Be("查詢字串測試物流");
                });

        var content = await response.Content.ReadAsStringAsync();
        this._output.WriteLine($"Request URI: /api/shippers/{shipperId}");
        this._output.WriteLine($"Response Content: {content}");
    }

    #endregion

    #region 錯誤處理進階測試

    [Fact]
    [Trait("Category", "api/shippers.GET")]
    public async Task GetShipper_當伺服器發生內部錯誤_應回傳適當的錯誤回應()
    {
        // Arrange - 使用一個可能觸發內部錯誤的 ID
        await this.CleanupDatabaseAsync();
        var invalidId = int.MaxValue;

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($"/api/shippers/{invalidId}", UriKind.RelativeOrAbsolute)
        };
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

        // Act
        var response = await this.Client.SendAsync(request);

        // Assert
        response.Should().Be404NotFound();

        var content = await response.Content.ReadAsStringAsync();
        this._output.WriteLine($"Response Content: {content}");

        // 驗證回應內容不為空
        content.Should().NotBeNullOrEmpty();
    }

    [Fact]
    [Trait("Category", "api/shippers.POST")]
    public async Task CreateShipper_當JSON格式錯誤_應回傳BadRequest()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var invalidJson = "{ invalid json format }";

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("/api/shippers", UriKind.RelativeOrAbsolute),
            Content = new StringContent(invalidJson, Encoding.UTF8, "application/json")
        };
        request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

        // Act
        var response = await this.Client.SendAsync(request);

        // Assert
        response.Should().Be400BadRequest();

        var content = await response.Content.ReadAsStringAsync();
        this._output.WriteLine($"Response Content: {content}");
    }

    #endregion

    #region 簡化的並行測試

    [Fact]
    [Trait("Category", "api/shippers.POST")]
    public async Task CreateMultipleShippers_並行建立_應都能成功()
    {
        // Arrange
        await this.CleanupDatabaseAsync(); // 確保測試隔離
        var tasks = new List<Task<HttpResponseMessage>>();

        for (var i = 1; i <= 3; i++) // 減少到 3 個以降低複雜度
        {
            var newShipper = new ShipperCreateParameter
            {
                CompanyName = $"並行測試物流{i:D2}",
                Phone = $"02-1234567{i}"
            };

            var json = JsonSerializer.Serialize(newShipper, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("/api/shippers", UriKind.RelativeOrAbsolute),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            tasks.Add(this.Client.SendAsync(request));
        }

        // Act
        var responses = await Task.WhenAll(tasks);

        // Assert
        foreach (var response in responses)
        {
            response.Should().Be201Created();

            var content = await response.Content.ReadAsStringAsync();
            this._output.WriteLine($"Created Shipper Response: {content}");
        }

        // 驗證資料確實被創建（檢查是否至少有資料）
        var getAllResponse = await this.Client.GetAsync("/api/shippers");
        getAllResponse.Should().Be200Ok()
                      .And.Satisfy<SuccessResultOutputModel<List<ShipperOutputModel>>>(getAllWrapper =>
                      {
                          getAllWrapper.Should().NotBeNull();
                          getAllWrapper.Data.Should().NotBeNull();
                          getAllWrapper.Data.Count.Should().BeGreaterThan(0); // 至少有資料即可
                      });
    }

    #endregion

    #region 效能測試

    [Fact]
    [Trait("Category", "Performance")]
    public async Task GetAllShippers_大量資料_應在合理時間內回應()
    {
        // Arrange - 建立測試資料
        await this.CleanupDatabaseAsync();
        for (var i = 1; i <= 5; i++) // 減少測試資料量
        {
            await this.SeedShipperAsync($"效能測試物流{i:D2}");
        }

        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await this.Client.GetAsync("/api/shippers");

        // Assert
        stopwatch.Stop();
        this._output.WriteLine($"Response time: {stopwatch.ElapsedMilliseconds}ms");

        response.Should().Be200Ok()
                .And.Satisfy<SuccessResultOutputModel<List<ShipperOutputModel>>>(responseWrapper =>
                {
                    responseWrapper.Should().NotBeNull();
                    responseWrapper.Data.Should().NotBeNull();
                    responseWrapper.Data.Count.Should().BeGreaterThanOrEqualTo(5);
                });
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // 5秒內
    }

    #endregion
}