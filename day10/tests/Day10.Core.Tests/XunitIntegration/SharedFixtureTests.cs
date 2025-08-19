namespace Day10.Core.Tests.XunitIntegration;

/// <summary>
/// 使用測試類別層級的 Fixture
/// </summary>
public class ProductServiceTestsWithSharedFixture
{
    private readonly Fixture _fixture;

    public ProductServiceTestsWithSharedFixture()
    {
        this._fixture = new Fixture();

        // 在建構式中進行共同的客製化設定
        this._fixture.Customize<ProductCreateRequest>(c => c.With(x => x.Price, () => (decimal)(Random.Shared.NextDouble() * 10000))   // 限制價格範圍
                                                            .With(x => x.Name, () => $"Product-{this._fixture.Create<string>()[..8]}") // 自訂名稱格式
        );
    }

    [Fact]
    public void CreateProduct_使用共享Fixture_應成功建立()
    {
        // Arrange
        var productData = this._fixture.Create<ProductCreateRequest>();
        var service = new ProductService();

        // Act
        var actual = service.CreateProduct(productData);

        // Assert
        Assert.NotNull(actual);
        Assert.True(productData.Price <= 10000);
        Assert.StartsWith("Product-", productData.Name);
    }

    [Fact]
    public void ValidateProduct_使用共享Fixture_應通過驗證()
    {
        // Arrange
        var productData = this._fixture.Create<ProductCreateRequest>();
        var validator = new ProductValidator();

        // Act
        var isValid = validator.Validate(productData);

        // Assert
        isValid.Should().BeTrue();
    }
}