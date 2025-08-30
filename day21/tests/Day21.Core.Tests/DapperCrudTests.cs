using Day21.Core.Repositories;
using Day21.Core.Tests.Infrastructure;

namespace Day21.Core.Tests;

/// <summary>
/// Dapper Repository CRUD 操作測試
/// </summary>
[Collection(nameof(SqlServerCollectionFixture))]
public class DapperCrudTests : IDisposable
{
    private readonly IDbConnection _connection;
    private readonly IProductRepository _productRepository;
    private readonly ITestOutputHelper _testOutputHelper;

    public DapperCrudTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        var connectionString = SqlServerContainerFixture.ConnectionString;
        _connection = new SqlConnection(connectionString);
        _connection.Open();

        _productRepository = new DapperProductRepository(connectionString);

        // 確保測試表格存在
        EnsureTablesExist();
    }

    public void Dispose()
    {
        // 清理測試資料
        _connection.Execute("DELETE FROM ProductTags");
        _connection.Execute("DELETE FROM OrderItems");
        _connection.Execute("DELETE FROM Orders");
        _connection.Execute("DELETE FROM Products");
        _connection.Execute("DELETE FROM Categories");
        _connection.Close();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }

    private void EnsureTablesExist()
    {
        var scriptDirectory = Path.Combine(AppContext.BaseDirectory, "SqlScripts");
        if (!Directory.Exists(scriptDirectory))
        {
            return;
        }

        // 按照依賴順序執行表格建立腳本
        var orderedScripts = new[]
        {
            "Tables/CreateCategoriesTable.sql",
            "Tables/CreateTagsTable.sql",
            "Tables/CreateCustomersTable.sql",
            "Tables/CreateProductsTable.sql",
            "Tables/CreateOrdersTable.sql",
            "Tables/CreateOrderItemsTable.sql",
            "Tables/CreateProductTagsTable.sql"
        };

        foreach (var scriptPath in orderedScripts)
        {
            var fullPath = Path.Combine(scriptDirectory, scriptPath);
            if (!File.Exists(fullPath))
            {
                continue;
            }

            var script = File.ReadAllText(fullPath);
            _connection.Execute(script);
        }

        // 建立測試分類
        var categoryExists = _connection.QuerySingle<int>("SELECT COUNT(*) FROM Categories");
        if (categoryExists == 0)
        {
            _connection.Execute("""
                                INSERT INTO Categories (Name, Description, IsActive) 
                                VALUES ('電子產品', '各種電子設備', 1), ('書籍', '各類書籍', 1)
                                """);
        }
    }

    [Fact]
    public async Task AddAsync_使用DapperRepository新增商品_應該成功儲存()
    {
        // Arrange
        var categoryId = await _connection.QuerySingleAsync<int>("SELECT TOP 1 Id FROM Categories WHERE IsActive = 1");
        var product = new Product
        {
            Name = "Dapper Repository 測試商品",
            Description = "Dapper Repo 測試用",
            Price = 2500m,
            Stock = 15,
            CategoryId = categoryId,
            SKU = "DAPPER-REPO-001",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _productRepository.AddAsync(product);

        // Assert
        product.Id.Should().BeGreaterThan(0);
        var savedProduct = await _productRepository.GetByIdAsync(product.Id);
        savedProduct.Should().NotBeNull();
        savedProduct.Name.Should().Be(product.Name);
    }

    [Fact]
    public async Task GetAllAsync_使用DapperRepository查詢所有商品_應該回傳所有商品()
    {
        // Arrange
        var categoryId = await _connection.QuerySingleAsync<int>("SELECT TOP 1 Id FROM Categories WHERE IsActive = 1");
        await _productRepository.AddAsync(new Product
        {
            Name = "商品1", Price = 100m, CategoryId = categoryId, SKU = "SKU1", IsActive = true, CreatedAt = DateTime.UtcNow
        });
        await _productRepository.AddAsync(new Product
        {
            Name = "商品2", Price = 200m, CategoryId = categoryId, SKU = "SKU2", IsActive = true, CreatedAt = DateTime.UtcNow
        });

        // Act
        var products = await _productRepository.GetAllAsync();

        // Assert
        products.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_使用DapperRepository查詢單一商品_應該回傳正確商品()
    {
        // Arrange
        var categoryId = await _connection.QuerySingleAsync<int>("SELECT TOP 1 Id FROM Categories WHERE IsActive = 1");
        var newProduct = new Product
        {
            Name = "查詢用商品",
            Price = 150m,
            CategoryId = categoryId,
            SKU = "SKU3",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _productRepository.AddAsync(newProduct);

        // Act
        var product = await _productRepository.GetByIdAsync(newProduct.Id);

        // Assert
        product.Should().NotBeNull();
        product!.Id.Should().Be(newProduct.Id);
        product.Name.Should().Be("查詢用商品");
    }

    [Fact]
    public async Task UpdateAsync_使用DapperRepository更新商品_應該成功更新()
    {
        // Arrange
        var categoryId = await _connection.QuerySingleAsync<int>("SELECT TOP 1 Id FROM Categories WHERE IsActive = 1");
        var productToUpdate = new Product
        {
            Name = "待更新商品",
            Price = 300m,
            CategoryId = categoryId,
            SKU = "SKU4",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _productRepository.AddAsync(productToUpdate);

        var product = await _productRepository.GetByIdAsync(productToUpdate.Id);
        product!.Name = "已更新商品";
        product.Price = 350m;

        // Act
        await _productRepository.UpdateAsync(product);

        // Assert
        var updatedProduct = await _productRepository.GetByIdAsync(productToUpdate.Id);
        updatedProduct.Should().NotBeNull();
        updatedProduct.Name.Should().Be("已更新商品");
        updatedProduct.Price.Should().Be(350m);
    }

    [Fact]
    public async Task DeleteAsync_使用DapperRepository刪除商品_應該成功刪除()
    {
        // Arrange
        var categoryId = await _connection.QuerySingleAsync<int>("SELECT TOP 1 Id FROM Categories WHERE IsActive = 1");
        var productToDelete = new Product
        {
            Name = "待刪除商品",
            Price = 400m,
            CategoryId = categoryId,
            SKU = "SKU5",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _productRepository.AddAsync(productToDelete);

        // Act
        await _productRepository.DeleteAsync(productToDelete.Id);

        // Assert
        var deletedProduct = await _productRepository.GetByIdAsync(productToDelete.Id);
        deletedProduct.Should().BeNull();
    }
}