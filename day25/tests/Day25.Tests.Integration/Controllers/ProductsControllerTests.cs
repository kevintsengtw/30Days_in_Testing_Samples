namespace Day25.Tests.Integration.Controllers;

/// <summary>
/// ProductsController 整合測試 - 使用 Aspire Testing
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class ProductsControllerTests : IntegrationTestBase
{
    public ProductsControllerTests(AspireAppFixture fixture) : base(fixture)
    {
    }

    #region GET /products 測試

    [Fact]
    public async Task GetProducts_當沒有產品時_應回傳空的分頁結果()
    {
        // Arrange
        // (資料庫應為空)

        // Act
        var response = await HttpClient.GetAsync("/products");

        // Assert
        response.Should().Be200Ok()
                .And.Satisfy<PagedResult<ProductResponse>>(result =>
                {
                    result.Total.Should().Be(0);
                    result.PageSize.Should().Be(10);
                    result.Page.Should().Be(1);
                    result.Items.Should().BeEmpty();
                });
    }

    [Fact]
    public async Task GetProducts_使用分頁參數_應回傳正確的分頁結果()
    {
        // Arrange
        await TestHelpers.SeedProductsAsync(DatabaseManager, 15);

        // Act - 使用 Flurl 建構 QueryString
        var url = "/products"
                  .SetQueryParam("pageSize", 5)
                  .SetQueryParam("page", 2);

        var response = await HttpClient.GetAsync(url);

        // Assert
        response.Should().Be200Ok()
                .And.Satisfy<PagedResult<ProductResponse>>(result =>
                {
                    result.Total.Should().Be(15);
                    result.PageSize.Should().Be(5);
                    result.Page.Should().Be(2);
                    result.PageCount.Should().Be(3);
                    result.Items.Should().HaveCount(5);
                    result.Items.Should().AllSatisfy(product =>
                    {
                        product.Id.Should().NotBeEmpty();
                        product.Name.Should().NotBeNullOrEmpty();
                        product.Price.Should().BeGreaterThan(0);
                    });
                });
    }

    [Fact]
    public async Task GetProducts_使用搜尋參數_應回傳符合條件的產品()
    {
        // Arrange
        await TestHelpers.SeedProductsAsync(DatabaseManager, 5);
        await TestHelpers.SeedSpecificProductAsync(DatabaseManager, "特殊產品", 199.99m);

        // Act - 使用 Flurl 建構 QueryString
        var url = "/products"
                  .SetQueryParam("keyword", "特殊")
                  .SetQueryParam("pageSize", 10);

        var response = await HttpClient.GetAsync(url);

        // Assert
        response.Should().Be200Ok()
                .And.Satisfy<PagedResult<ProductResponse>>(result =>
                {
                    result.Total.Should().Be(1);
                    result.Items.Should().HaveCount(1);

                    var product = result.Items.First();
                    product.Name.Should().Be("特殊產品");
                    product.Price.Should().Be(199.99m);
                });
    }

    #endregion

    #region POST /products 測試

    [Fact]
    public async Task CreateProduct_使用有效資料_應成功建立產品()
    {
        // Arrange
        var request = TestHelpers.CreateProductRequest("測試產品", 99.99m);

        // Act
        var response = await HttpClient.PostAsJsonAsync("/products", request);

        // Assert
        response.Should().Be201Created()
                .And.Satisfy<ProductResponse>(product =>
                {
                    product.Id.Should().NotBeEmpty();
                    product.Name.Should().Be("測試產品");
                    product.Price.Should().Be(99.99m);
                    product.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1));
                    product.UpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1));
                });

        // 確認 Location header
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateProduct_當產品名稱為空_應回傳400BadRequest()
    {
        // Arrange
        var invalidRequest = new ProductCreateRequest { Name = "", Price = 100.00m };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/products", invalidRequest);

        // Assert
        response.Should().Be400BadRequest()
                .And.Satisfy<ValidationProblemDetails>(problem =>
                {
                    problem.Title.Should().Be("One or more validation errors occurred.");
                    problem.Status.Should().Be(400);
                    problem.Errors.Should().ContainKey("Name");
                    problem.Errors["Name"].Should().Contain("產品名稱不能為空");
                });
    }

    [Fact]
    public async Task CreateProduct_當產品名稱和價格都無效_應回傳400BadRequest()
    {
        // Arrange
        var invalidRequest = new ProductCreateRequest { Name = "", Price = -10.00m };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/products", invalidRequest);

        // Assert
        response.Should().Be400BadRequest()
                .And.Satisfy<ValidationProblemDetails>(problem =>
                {
                    problem.Errors.Should().ContainKey("Name");
                    problem.Errors.Should().ContainKey("Price");
                    problem.Errors["Name"].Should().Contain("產品名稱不能為空");
                    problem.Errors["Price"].Should().Contain("產品價格必須大於 0");
                });
    }

    #endregion

    #region GET /products/{id} 測試

    [Fact]
    public async Task GetById_當產品存在_應回傳產品資料()
    {
        // Arrange
        var productId = await TestHelpers.SeedSpecificProductAsync(DatabaseManager, "測試產品", 149.99m);

        // Act
        var response = await HttpClient.GetAsync($"/products/{productId}");

        // Assert
        response.Should().Be200Ok()
                .And.Satisfy<ProductResponse>(product =>
                {
                    product.Id.Should().Be(productId);
                    product.Name.Should().Be("測試產品");
                    product.Price.Should().Be(149.99m);
                });
    }

    [Fact]
    public async Task GetById_當產品不存在_應回傳404且包含ProblemDetails()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await HttpClient.GetAsync($"/products/{nonExistentId}");

        // Assert
        response.Should().Be404NotFound()
                .And.Satisfy<ProblemDetails>(problemDetails =>
                {
                    problemDetails.Title.Should().Be("資源不存在");
                    problemDetails.Status.Should().Be(404);
                    problemDetails.Type.Should().Be("https://httpstatuses.com/404");
                });

        // 檢查回應內容中包含 ID
        var content = await response.Content.ReadAsStringAsync();
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(content);
        jsonElement.GetProperty("detail").GetString().Should().Contain($"找不到 ID 為 {nonExistentId} 的產品");
    }

    #endregion

    #region PUT /products/{id} 測試

    [Fact]
    public async Task UpdateProduct_使用有效資料_應成功更新產品()
    {
        // Arrange
        var productId = await TestHelpers.SeedSpecificProductAsync(DatabaseManager, "原始產品", 100.00m);
        var updateRequest = new ProductUpdateRequest { Name = "更新後產品", Price = 150.00m };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/products/{productId}", updateRequest);

        // Assert
        response.Should().Be204NoContent();

        // 驗證產品已更新
        var getResponse = await HttpClient.GetAsync($"/products/{productId}");
        getResponse.Should().Be200Ok()
                   .And.Satisfy<ProductResponse>(product =>
                   {
                       product.Name.Should().Be("更新後產品");
                       product.Price.Should().Be(150.00m);
                       product.UpdatedAt.Should().BeAfter(product.CreatedAt);
                   });
    }

    [Fact]
    public async Task UpdateProduct_當產品不存在_應回傳404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateRequest = new ProductUpdateRequest { Name = "不存在的產品", Price = 100.00m };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/products/{nonExistentId}", updateRequest);

        // Assert
        response.Should().Be404NotFound();
    }

    #endregion

    #region DELETE /products/{id} 測試

    [Fact]
    public async Task DeleteProduct_當產品存在_應成功刪除()
    {
        // Arrange
        var productId = await TestHelpers.SeedSpecificProductAsync(DatabaseManager, "要刪除的產品", 99.99m);

        // Act
        var response = await HttpClient.DeleteAsync($"/products/{productId}");

        // Assert
        response.Should().Be204NoContent();

        // 確認產品已被刪除
        var getResponse = await HttpClient.GetAsync($"/products/{productId}");
        getResponse.Should().Be404NotFound();
    }

    [Fact]
    public async Task Delete_當產品不存在_應回傳404且包含ProblemDetails()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await HttpClient.DeleteAsync($"/products/{nonExistentId}");

        // Assert
        response.Should().Be404NotFound()
                .And.Satisfy<ProblemDetails>(problemDetails =>
                {
                    problemDetails.Title.Should().Be("資源不存在");
                    problemDetails.Status.Should().Be(404);
                });

        // 檢查詳細錯誤訊息
        var content = await response.Content.ReadAsStringAsync();
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(content);
        jsonElement.GetProperty("detail").GetString().Should().Contain($"找不到 ID 為 {nonExistentId} 的產品");
    }

    #endregion
}