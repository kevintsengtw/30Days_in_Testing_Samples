using Day21.Core.Repositories;
using Day21.Core.Tests.Infrastructure;

namespace Day21.Core.Tests;

/// <summary>
/// EF Core Repository CRUD 操作測試
/// </summary>
[Collection(nameof(SqlServerCollectionFixture))]
public class EfCoreCrudTests : IDisposable
{
    private readonly ECommerceDbContext _dbContext;
    private readonly IProductRepository _productRepository;
    private readonly ITestOutputHelper _testOutputHelper;

    /// <summary>
    /// 建構式，初始化 DbContext 和 Repository
    /// </summary>
    /// <param name="testOutputHelper"></param>
    public EfCoreCrudTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;

        var connectionString = SqlServerContainerFixture.ConnectionString;
        _testOutputHelper.WriteLine($"使用連線字串：{connectionString}");

        // 建立 EF Core DbContext
        var options = new DbContextOptionsBuilder<ECommerceDbContext>()
                      .UseSqlServer(connectionString)
                      .EnableSensitiveDataLogging()
                      .LogTo(_testOutputHelper.WriteLine, LogLevel.Information)
                      .Options;

        _dbContext = new ECommerceDbContext(options);
        _productRepository = new EfCoreProductRepository(_dbContext);

        // 使用 SQL 腳本建立表格，而不是 EnsureCreated()
        EnsureTablesExist();
    }

    /// <summary>
    /// 確保資料表存在，若不存在則執行 SQL 腳本建立
    /// </summary>
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
            _dbContext.Database.ExecuteSqlRaw(script);
        }
    }

    /// <summary>
    /// 清理測試資料
    /// </summary>
    public void Dispose()
    {
        _dbContext.Database.ExecuteSqlRaw("DELETE FROM ProductTags");
        _dbContext.Database.ExecuteSqlRaw("DELETE FROM OrderItems");
        _dbContext.Database.ExecuteSqlRaw("DELETE FROM Orders");
        _dbContext.Database.ExecuteSqlRaw("DELETE FROM Products");
        _dbContext.Database.ExecuteSqlRaw("DELETE FROM Categories");
        _dbContext.Database.ExecuteSqlRaw("DELETE FROM Tags");
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 預先建立一個分類，供商品測試使用
    /// </summary>
    private async Task SeedCategoryAsync()
    {
        if (!await _dbContext.Categories.AnyAsync())
        {
            _dbContext.Categories.Add(new Category
            {
                Name = "電子產品",
                Description = "各種電子設備",
                IsActive = true
            });
            await _dbContext.SaveChangesAsync();
        }
    }

    [Fact]
    public async Task AddAsync_使用EfCoreRepository新增商品_應該成功儲存()
    {
        // Arrange
        await SeedCategoryAsync();
        var category = await _dbContext.Categories.FirstAsync();
        var product = new Product
        {
            Name = "EF Core Repo 測試商品",
            Description = "這是一個測試商品",
            Price = 1500,
            Stock = 25,
            CategoryId = category.Id,
            SKU = "EFCORE-REPO-001",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _productRepository.AddAsync(product);

        // Assert
        product.Id.Should().BeGreaterThan(0);
        var saved = await _dbContext.Products.FindAsync(product.Id);
        saved.Should().NotBeNull();
        saved.Name.Should().Be("EF Core Repo 測試商品");
    }

    [Fact]
    public async Task GetAllAsync_使用EfCoreRepository查詢所有商品_應回傳所有商品()
    {
        // Arrange
        await SeedCategoryAsync();
        var category = await _dbContext.Categories.FirstAsync();
        await _productRepository.AddAsync(new Product
        {
            Name = "商品1", Price = 100, CategoryId = category.Id, SKU = "SKU1", IsActive = true, CreatedAt = DateTime.UtcNow
        });
        await _productRepository.AddAsync(new Product
        {
            Name = "商品2", Price = 200, CategoryId = category.Id, SKU = "SKU2", IsActive = true, CreatedAt = DateTime.UtcNow
        });

        // Act
        var products = await _productRepository.GetAllAsync();

        // Assert
        products.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_使用EfCoreRepository查詢單一商品_應回傳正確商品()
    {
        // Arrange
        await SeedCategoryAsync();
        var category = await _dbContext.Categories.FirstAsync();
        var newProduct = new Product
        {
            Name = "查詢用商品",
            Price = 150,
            CategoryId = category.Id,
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
    public async Task UpdateAsync_使用EfCoreRepository更新商品_應成功更新()
    {
        // Arrange
        await SeedCategoryAsync();
        var category = await _dbContext.Categories.FirstAsync();
        var productToUpdate = new Product
        {
            Name = "待更新商品",
            Price = 300,
            CategoryId = category.Id,
            SKU = "SKU4",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _productRepository.AddAsync(productToUpdate);

        _dbContext.ChangeTracker.Clear(); // 清除追蹤，模擬從不同上下文中取得並更新

        var product = await _productRepository.GetByIdAsync(productToUpdate.Id);
        product!.Name = "已更新商品";
        product.Price = 350;

        // Act
        await _productRepository.UpdateAsync(product);

        // Assert
        var updatedProduct = await _dbContext.Products.FindAsync(productToUpdate.Id);
        updatedProduct.Should().NotBeNull();
        updatedProduct!.Name.Should().Be("已更新商品");
        updatedProduct.Price.Should().Be(350);
    }

    [Fact]
    public async Task DeleteAsync_使用EfCoreRepository刪除商品_應成功刪除()
    {
        // Arrange
        await SeedCategoryAsync();
        var category = await _dbContext.Categories.FirstAsync();
        var productToDelete = new Product
        {
            Name = "待刪除商品",
            Price = 400,
            CategoryId = category.Id,
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