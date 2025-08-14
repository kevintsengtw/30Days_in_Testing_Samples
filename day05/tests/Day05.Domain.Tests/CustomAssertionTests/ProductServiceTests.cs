using Day05.Domain.Services.BusinessServices;
using Day05.Tests.Extensions;

namespace Day05.Domain.Tests.CustomAssertionTests;

// 使用自訂 Assertions
public class ProductServiceTests
{
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        this._productService = new ProductService();
    }

    [Fact]
    public void CreateProduct_有效資料_應該回傳有效產品()
    {
        var actual = this._productService.Create("Laptop", 999.99m);

        // 使用領域特定 Assertions
        actual.Should().BeValidProduct();
        actual.Name.Should().Be("Laptop");
    }
}