using Day23.Integration.Tests.Infrastructure;
using Flurl;
using Microsoft.AspNetCore.Mvc;

namespace Day23.Integration.Tests.Controllers;

/// <summary>
/// ProductsController 整合測試
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class ProductsControllerTests : IntegrationTestBase
{
    public ProductsControllerTests(TestWebApplicationFactory factory) : base(factory)
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
        var request = TestHelpers.CreateProductRequest("新產品", 299.99m);

        // Act
        var response = await HttpClient.PostAsJsonAsync("/products", request);

        // Assert
        response.Should().Be201Created()
                .And.Satisfy<ProductResponse>(product =>
                {
                    product.Id.Should().NotBeEmpty();
                    product.Name.Should().Be("新產品");
                    product.Price.Should().Be(299.99m);
                    product.CreatedAt.Should().Be(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
                    product.UpdatedAt.Should().Be(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
                });
    }

    [Fact]
    public async Task CreateProduct_當產品名稱為空_應回傳400BadRequest()
    {
        // Arrange
        var invalidRequest = new ProductCreateRequest
        {
            Name = "",
            Price = 100.00m
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/products", invalidRequest);

        // Assert
        response.Should().Be400BadRequest()
                .And.Satisfy<ValidationProblemDetails>(problem =>
                {
                    problem.Type.Should().Be("https://tools.ietf.org/html/rfc9110#section-15.5.1");
                    problem.Title.Should().Be("One or more validation errors occurred.");
                    problem.Status.Should().Be(400);
                    problem.Errors.Should().ContainKey("Name");
                    problem.Errors["Name"].Should().Contain("產品名稱不能為空");
                });
    }

    [Fact]
    public async Task CreateProduct_當產品名稱超過長度限制_應回傳400BadRequest()
    {
        // Arrange
        var invalidRequest = new ProductCreateRequest
        {
            Name = new string('A', 101), // 超過 100 字元限制
            Price = 100.00m
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/products", invalidRequest);

        // Assert
        response.Should().Be400BadRequest()
                .And.Satisfy<ValidationProblemDetails>(problem =>
                {
                    problem.Type.Should().Be("https://tools.ietf.org/html/rfc9110#section-15.5.1");
                    problem.Title.Should().Be("One or more validation errors occurred.");
                    problem.Status.Should().Be(400);
                    problem.Errors.Should().ContainKey("Name");
                    problem.Errors["Name"].Should().Contain("產品名稱不能超過 100 個字元");
                });
    }

    [Fact]
    public async Task CreateProduct_當產品價格為負數_應回傳400BadRequest()
    {
        // Arrange
        var invalidRequest = new ProductCreateRequest
        {
            Name = "有效產品名稱",
            Price = -10.00m
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/products", invalidRequest);

        // Assert
        response.Should().Be400BadRequest()
                .And.Satisfy<ValidationProblemDetails>(problem =>
                {
                    problem.Type.Should().Be("https://tools.ietf.org/html/rfc9110#section-15.5.1");
                    problem.Title.Should().Be("One or more validation errors occurred.");
                    problem.Status.Should().Be(400);
                    problem.Errors.Should().ContainKey("Price");
                    problem.Errors["Price"].Should().Contain("產品價格必須大於 0");
                });
    }

    [Fact]
    public async Task CreateProduct_當產品價格超過上限_應回傳400BadRequest()
    {
        // Arrange
        var invalidRequest = new ProductCreateRequest
        {
            Name = "有效產品名稱",
            Price = 1000000.00m // 超過 999,999.99 上限
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/products", invalidRequest);

        // Assert
        response.Should().Be400BadRequest()
                .And.Satisfy<ValidationProblemDetails>(problem =>
                {
                    problem.Status.Should().Be(400);
                    problem.Errors.Should().ContainKey("Price");
                    problem.Errors["Price"].Should().Contain("產品價格不能超過 999,999.99");
                });
    }

    [Fact]
    public async Task CreateProduct_當產品名稱為null_應回傳400BadRequest()
    {
        // Arrange
        var invalidRequest = new ProductCreateRequest
        {
            Name = null!, // null 應觸發驗證錯誤
            Price = 100.00m
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/products", invalidRequest);

        // Assert
        response.Should().Be400BadRequest()
                .And.Satisfy<ValidationProblemDetails>(problem =>
                {
                    problem.Type.Should().Be("https://tools.ietf.org/html/rfc9110#section-15.5.1");
                    problem.Title.Should().Be("One or more validation errors occurred.");
                    problem.Status.Should().Be(400);
                    problem.Errors.Should().ContainKey("Name");
                    problem.Errors["Name"].Should().Contain("產品名稱不能為空");
                });
    }

    [Fact]
    public async Task CreateProduct_當產品價格為0_應回傳400BadRequest()
    {
        // Arrange
        var invalidRequest = new ProductCreateRequest
        {
            Name = "有效產品名稱",
            Price = 0m // 價格為 0 應觸發驗證錯誤
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/products", invalidRequest);

        // Assert
        response.Should().Be400BadRequest()
                .And.Satisfy<ValidationProblemDetails>(problem =>
                {
                    problem.Type.Should().Be("https://tools.ietf.org/html/rfc9110#section-15.5.1");
                    problem.Title.Should().Be("One or more validation errors occurred.");
                    problem.Status.Should().Be(400);
                    problem.Errors.Should().ContainKey("Price");
                    problem.Errors["Price"].Should().Contain("產品價格必須大於 0");
                });
    }

    [Fact]
    public async Task CreateProduct_當產品名稱和價格都無效_應回傳400BadRequest()
    {
        // Arrange
        var invalidRequest = new ProductCreateRequest
        {
            Name = "",
            Price = -10.00m
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/products", invalidRequest);

        // Assert
        response.Should().Be400BadRequest()
                .And.Satisfy<ValidationProblemDetails>(problem =>
                {
                    problem.Status.Should().Be(400);
                    problem.Errors.Should().ContainKey("Name");
                    problem.Errors.Should().ContainKey("Price");
                    problem.Errors["Name"].Should().Contain("產品名稱不能為空");
                    problem.Errors["Price"].Should().Contain("產品價格必須大於 0");
                });
    }

    #endregion

    #region GET /products/{id} 測試

    [Fact]
    public async Task GetProductById_當產品存在_應回傳產品資料()
    {
        // Arrange
        var productId = await TestHelpers.SeedSpecificProductAsync(DatabaseManager, "測試產品", 199.99m);

        // Act
        var response = await HttpClient.GetAsync($"/products/{productId}");

        // Assert
        response.Should().Be200Ok()
                .And.Satisfy<ProductResponse>(product =>
                {
                    product.Id.Should().Be(productId);
                    product.Name.Should().Be("測試產品");
                    product.Price.Should().Be(199.99m);
                    product.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1));
                    product.UpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1));
                });
    }

    [Fact]
    public async Task GetProductById_當產品不存在_應回傳404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await HttpClient.GetAsync($"/products/{nonExistentId}");

        // Assert
        response.Should().Be404NotFound()
                .And.Satisfy<ProblemDetails>(problem =>
                {
                    problem.Type.Should().Be("https://httpstatuses.com/404");
                    problem.Title.Should().Be("產品不存在");
                    problem.Status.Should().Be(404);
                    problem.Detail.Should().Contain($"找不到 ID 為 {nonExistentId} 的產品");
                });
    }

    #endregion

    #region PUT /products/{id} 測試

    [Fact]
    public async Task UpdateProduct_使用有效資料_應成功更新產品()
    {
        // Arrange
        var productId = await TestHelpers.SeedSpecificProductAsync(DatabaseManager, "原始產品", 100.00m);
        var updateRequest = new ProductUpdateRequest
        {
            Name = "更新的產品",
            Price = 150.00m
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/products/{productId}", updateRequest);

        // Assert
        response.Should().Be204NoContent();

        // 驗證產品已更新
        var getResponse = await HttpClient.GetAsync($"/products/{productId}");
        getResponse.Should().Be200Ok()
                   .And.Satisfy<ProductResponse>(product =>
                   {
                       product.Name.Should().Be("更新的產品");
                       product.Price.Should().Be(150.00m);
                   });
    }

    [Fact]
    public async Task UpdateProduct_當產品不存在_應回傳404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateRequest = new ProductUpdateRequest
        {
            Name = "更新的產品名稱",
            Price = 150.00m
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/products/{nonExistentId}", updateRequest);

        // Assert
        response.Should().Be404NotFound()
                .And.Satisfy<ProblemDetails>(problem =>
                {
                    problem.Type.Should().Be("https://httpstatuses.com/404");
                    problem.Title.Should().Be("產品不存在");
                    problem.Status.Should().Be(404);
                    problem.Detail.Should().Contain($"找不到 ID 為 {nonExistentId} 的產品");
                });
    }

    [Fact]
    public async Task UpdateProduct_當產品名稱為空_應回傳400BadRequest()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var invalidRequest = new ProductUpdateRequest
        {
            Name = "",
            Price = 100.00m
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/products/{productId}", invalidRequest);

        // Assert
        response.Should().Be400BadRequest()
                .And.Satisfy<ValidationProblemDetails>(problem =>
                {
                    problem.Status.Should().Be(400);
                    problem.Errors.Should().ContainKey("Name");
                    problem.Errors["Name"].Should().Contain("產品名稱不能為空");
                });
    }

    [Fact]
    public async Task UpdateProduct_當產品價格無效_應回傳400BadRequest()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var invalidRequest = new ProductUpdateRequest
        {
            Name = "有效產品名稱",
            Price = -10.00m
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/products/{productId}", invalidRequest);

        // Assert
        response.Should().Be400BadRequest()
                .And.Satisfy<ValidationProblemDetails>(problem =>
                {
                    problem.Status.Should().Be(400);
                    problem.Errors.Should().ContainKey("Price");
                    problem.Errors["Price"].Should().Contain("產品價格必須大於 0");
                });
    }

    #endregion

    #region DELETE /products/{id} 測試

    [Fact]
    public async Task DeleteProduct_當產品存在_應成功刪除產品()
    {
        // Arrange
        var productId = await TestHelpers.SeedSpecificProductAsync(DatabaseManager, "待刪除產品", 99.99m);

        // Act
        var response = await HttpClient.DeleteAsync($"/products/{productId}");

        // Assert
        response.Should().Be204NoContent();

        // 驗證產品已刪除
        var getResponse = await HttpClient.GetAsync($"/products/{productId}");
        getResponse.Should().Be404NotFound();
    }

    [Fact]
    public async Task DeleteProduct_當產品不存在_應回傳404NotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await HttpClient.DeleteAsync($"/products/{nonExistentId}");

        // Assert
        response.Should().Be404NotFound()
                .And.Satisfy<ProblemDetails>(problem =>
                {
                    problem.Type.Should().Be("https://httpstatuses.com/404");
                    problem.Title.Should().Be("產品不存在");
                    problem.Status.Should().Be(404);
                    problem.Detail.Should().Contain($"找不到 ID 為 {nonExistentId} 的產品");
                });
    }

    #endregion
}