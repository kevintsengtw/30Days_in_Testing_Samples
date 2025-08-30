using System.Diagnostics;
using Day21.Core.Repositories;
using Day21.Core.Tests.Infrastructure;

namespace Day21.Core.Tests;

/// <summary>
/// EF Core 進階功能的整合測試。
/// </summary>
[Collection(nameof(SqlServerCollectionFixture))]
public class EfCoreAdvancedTests : IDisposable
{
    private readonly ECommerceDbContext _dbContext;
    private readonly IProductByEFCoreRepository _advancedRepository;
    private readonly ITestOutputHelper _testOutputHelper;

    public EfCoreAdvancedTests(ITestOutputHelper testOutputHelper)
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

        // 使用 SQL 腳本建立表格，而不是 EnsureCreated()
        EnsureTablesExist();

        // 注入 EF Core 的進階 Repository 實作
        _advancedRepository = new EfCoreProductRepository(_dbContext);
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
            _dbContext.Database.ExecuteSqlRaw(script);
        }

        // 建立預存程序
        var storedProceduresDirectory = Path.Combine(scriptDirectory, "StoredProcedures");
        if (Directory.Exists(storedProceduresDirectory))
        {
            var spScriptFiles = Directory.GetFiles(storedProceduresDirectory, "*.sql");
            foreach (var scriptFile in spScriptFiles)
            {
                var script = File.ReadAllText(scriptFile);
                _dbContext.Database.ExecuteSqlRaw(script);
            }
        }
    }

    [Fact]
    public async Task GetProductWithCategoryAndTagsAsync_載入完整關聯資料_應該正確載入所有相關資料()
    {
        // Arrange
        var category = new Category { Name = "進階測試分類" };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        var tag1 = new Tag { Name = "標籤1" };
        var tag2 = new Tag { Name = "標籤2" };
        _dbContext.Tags.Add(tag1);
        _dbContext.Tags.Add(tag2);
        await _dbContext.SaveChangesAsync();

        var product = new Product
        {
            Name = "測試產品",
            Price = 1000,
            CategoryId = category.Id,
            SKU = "ADV-TEST-001",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        var productTag1 = new ProductTag { ProductId = product.Id, TagId = tag1.Id };
        var productTag2 = new ProductTag { ProductId = product.Id, TagId = tag2.Id };
        _dbContext.ProductTags.AddRange(productTag1, productTag2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _advancedRepository.GetProductWithCategoryAndTagsAsync(product.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
        result.Category.Should().NotBeNull();
        result.Category.Name.Should().Be("進階測試分類");
        result.ProductTags.Should().HaveCount(2);
        result.ProductTags.Should().AllSatisfy(pt => pt.Tag.Should().NotBeNull());
    }

    [Fact]
    public async Task GetProductsByCategoryWithSplitQueryAsync_使用分割查詢_應該避免笛卡兒積問題()
    {
        // Arrange
        var category = new Category { Name = "分割查詢分類" };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        // 建立多個產品和標籤來模擬複雜關聯
        for (var i = 1; i <= 3; i++)
        {
            var product = new Product
            {
                Name = $"分割查詢產品{i}",
                Price = 100 * i,
                CategoryId = category.Id,
                SKU = $"SPLIT-{i:000}",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            // 每個產品加入多個標籤
            for (var j = 1; j <= 2; j++)
            {
                var tag = new Tag { Name = $"標籤{i}-{j}" };
                _dbContext.Tags.Add(tag);
                await _dbContext.SaveChangesAsync();

                var productTag = new ProductTag { ProductId = product.Id, TagId = tag.Id };
                _dbContext.ProductTags.Add(productTag);
            }
        }

        await _dbContext.SaveChangesAsync();

        // Act
        var results = await _advancedRepository.GetProductsByCategoryWithSplitQueryAsync(category.Id);

        // Assert
        results.Should().HaveCount(3);
        results.Should().AllSatisfy(p =>
        {
            p.Category.Should().NotBeNull();
            p.ProductTags.Should().HaveCount(2);
            p.ProductTags.Should().AllSatisfy(pt => pt.Tag.Should().NotBeNull());
        });
    }

    [Fact]
    public async Task BatchUpdateProductPricesAsync_批次更新價格_應該正確更新所有符合條件的產品()
    {
        // Arrange
        var category = new Category { Name = "批次更新分類" };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        var products = new[]
        {
            new Product { Name = "批次商品1", Price = 100, CategoryId = category.Id, SKU = "BATCH-1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Product { Name = "批次商品2", Price = 200, CategoryId = category.Id, SKU = "BATCH-2", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Product { Name = "批次商品3", Price = 300, CategoryId = category.Id, SKU = "BATCH-3", IsActive = false, CreatedAt = DateTime.UtcNow }
        };
        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Act
        var affectedRows = await _advancedRepository.BatchUpdateProductPricesAsync(category.Id, 1.1m);

        // Assert
        affectedRows.Should().Be(2); // 只有活躍的產品會被更新

        // 清除 DbContext 快取以確保從資料庫讀取最新資料
        _dbContext.ChangeTracker.Clear();

        var updatedProducts = await _dbContext.Products
                                              .Where(p => p.CategoryId == category.Id && p.IsActive)
                                              .ToListAsync();

        updatedProducts.Should().HaveCount(2);
        updatedProducts[0].Price.Should().Be(110m);
        updatedProducts[1].Price.Should().Be(220m);
    }

    [Fact]
    public async Task BatchDeleteInactiveProductsAsync_批次刪除非活躍產品_應該正確刪除符合條件的產品()
    {
        // Arrange
        var category = new Category { Name = "批次刪除分類" };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        var products = new[]
        {
            new Product { Name = "活躍商品", Price = 100, CategoryId = category.Id, SKU = "ACTIVE-1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Product { Name = "非活躍商品1", Price = 200, CategoryId = category.Id, SKU = "INACTIVE-1", IsActive = false, CreatedAt = DateTime.UtcNow },
            new Product { Name = "非活躍商品2", Price = 300, CategoryId = category.Id, SKU = "INACTIVE-2", IsActive = false, CreatedAt = DateTime.UtcNow }
        };
        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Act
        var deletedRows = await _advancedRepository.BatchDeleteInactiveProductsAsync(category.Id);

        // Assert
        deletedRows.Should().Be(2);

        var remainingProducts = await _dbContext.Products
                                                .Where(p => p.CategoryId == category.Id)
                                                .ToListAsync();

        remainingProducts.Should().HaveCount(1);
        remainingProducts[0].IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetProductsWithNoTrackingAsync_使用無追蹤查詢_應該提升查詢效能()
    {
        // Arrange
        var category = new Category { Name = "效能測試分類" };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        var products = new[]
        {
            new Product { Name = "低價商品", Price = 50, CategoryId = category.Id, SKU = "LOW-1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Product { Name = "高價商品1", Price = 150, CategoryId = category.Id, SKU = "HIGH-1", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Product { Name = "高價商品2", Price = 250, CategoryId = category.Id, SKU = "HIGH-2", IsActive = true, CreatedAt = DateTime.UtcNow }
        };
        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Act
        var results = await _advancedRepository.GetProductsWithNoTrackingAsync(100m);

        // Assert
        results.Should().HaveCount(2);
        results.Should().AllSatisfy(p =>
        {
            p.Price.Should().BeGreaterThanOrEqualTo(100m);
            p.IsActive.Should().BeTrue();
        });

        // 驗證返回的實體不會被 DbContext 追蹤
        foreach (var product in results)
        {
            var entry = _dbContext.Entry(product);
            entry.State.Should().Be(EntityState.Detached);
        }
    }

    [Fact]
    public async Task GetCategoryWithProductsAsync_Include一對多關聯查詢_應該正確載入相關資料()
    {
        // Arrange
        var category = new Category { Name = "3C產品", IsActive = true, CreatedAt = DateTime.UtcNow };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        var products = new[]
        {
            new Product { Name = "筆記型電腦", Price = 45000, CategoryId = category.Id, SKU = "LAPTOP001", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Product { Name = "滑鼠", Price = 1000, CategoryId = category.Id, SKU = "MOUSE001", IsActive = true, CreatedAt = DateTime.UtcNow }
        };
        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _advancedRepository.GetCategoryWithProductsAsync(category.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Products.Should().HaveCount(2);
        result.Products.Should().AllSatisfy(p => p.CategoryId.Should().Be(category.Id));
    }

    [Fact]
    public async Task GetProductWithMultiLevelIncludesAsync_Include多層關聯查詢_應該正確載入巢狀資料()
    {
        // Arrange
        var category = new Category { Name = "電子書", IsActive = true, CreatedAt = DateTime.UtcNow };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        var product = new Product
        {
            Name = "程式設計指南",
            Price = 500,
            CategoryId = category.Id,
            SKU = "EBOOK001",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        // 建立標籤
        var tags = new[]
        {
            new Tag { Name = "程式設計", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Tag { Name = "教育", IsActive = true, CreatedAt = DateTime.UtcNow }
        };
        _dbContext.Tags.AddRange(tags);
        await _dbContext.SaveChangesAsync();

        // 建立商品標籤關聯
        var productTags = new[]
        {
            new ProductTag { ProductId = product.Id, TagId = tags[0].Id },
            new ProductTag { ProductId = product.Id, TagId = tags[1].Id }
        };
        _dbContext.ProductTags.AddRange(productTags);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _advancedRepository.GetProductWithMultiLevelIncludesAsync(product.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Category.Should().NotBeNull();
        result.Category!.Name.Should().Be("電子書");
        result.ProductTags.Should().HaveCount(2);
        result.ProductTags.Should().Contain(pt => pt.Tag.Name == "程式設計");
        result.ProductTags.Should().Contain(pt => pt.Tag.Name == "教育");
    }

    [Fact]
    public async Task GetOrderWithComplexIncludesAsync_ThenInclude複雜多層關聯_應該正確載入所有層級資料()
    {
        // Arrange
        var category = new Category { Name = "書籍", IsActive = true, CreatedAt = DateTime.UtcNow };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        var product = new Product
        {
            Name = "測試驅動開發",
            Price = 800,
            CategoryId = category.Id,
            SKU = "TDD001",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        var customer = new Customer
        {
            Name = "測試客戶",
            Email = "test@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync();

        var order = new Order
        {
            OrderNumber = "ORD2024001",
            CustomerId = customer.Id,
            CustomerName = "測試客戶",
            CustomerEmail = "test@example.com",
            TotalAmount = 800,
            OrderDate = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();

        var orderItem = new OrderItem
        {
            OrderId = order.Id,
            ProductId = product.Id,
            Quantity = 1,
            UnitPrice = 800,
            Subtotal = 800,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.OrderItems.Add(orderItem);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _advancedRepository.GetOrderWithComplexIncludesAsync(order.Id);

        // Assert
        result.Should().NotBeNull();
        result!.OrderItems.Should().HaveCount(1);

        var orderItemResult = result.OrderItems.First();
        orderItemResult.Product.Should().NotBeNull();
        orderItemResult.Product.Name.Should().Be("測試驅動開發");
        orderItemResult.Product.Category.Should().NotBeNull();
        orderItemResult.Product.Category.Name.Should().Be("書籍");
    }

    [Fact]
    public async Task BatchApplyDiscountAsync_ExecuteUpdate批次更新關聯資料_應該高效更新()
    {
        // Arrange
        var category = new Category { Name = "特價商品", IsActive = true, CreatedAt = DateTime.UtcNow };
        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync();

        var products = new[]
        {
            new Product { Name = "商品A", Price = 1000, CategoryId = category.Id, SKU = "SALE001", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Product { Name = "商品B", Price = 2000, CategoryId = category.Id, SKU = "SALE002", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Product { Name = "商品C", Price = 3000, CategoryId = category.Id, SKU = "SALE003", IsActive = true, CreatedAt = DateTime.UtcNow }
        };
        _dbContext.Products.AddRange(products);
        await _dbContext.SaveChangesAsync();

        // Act - 批次調整特定分類下所有商品的價格（8折）
        var discountPercentage = 0.8m;
        var affectedRows = await _advancedRepository.BatchApplyDiscountAsync(category.Id, discountPercentage);

        // Assert
        affectedRows.Should().Be(3);

        // 清除 DbContext 快取以確保從資料庫讀取最新資料
        _dbContext.ChangeTracker.Clear();

        var updatedProducts = await _dbContext.Products
                                              .AsNoTracking()
                                              .Where(p => p.CategoryId == category.Id)
                                              .ToListAsync();

        updatedProducts.Should().HaveCount(3);
        updatedProducts[0].Price.Should().Be(800m);  // 1000 * 0.8
        updatedProducts[1].Price.Should().Be(1600m); // 2000 * 0.8
        updatedProducts[2].Price.Should().Be(2400m); // 3000 * 0.8
    }

    [Fact]
    public async Task N1QueryProblemVerification_對比有問題與最佳化的Repository方法_應該展示查詢效率差異()
    {
        // Arrange - 建立測試資料
        await CreateCategoriesWithProductsAsync();
        var stopwatch = new Stopwatch();

        // Act 1: 測試有問題的方法
        stopwatch.Start();
        var categoriesWithProblem = await _advancedRepository.GetCategoriesWithN1ProblemAsync();
        stopwatch.Stop();
        var problemTime = stopwatch.ElapsedMilliseconds;

        // Act 2: 測試最佳化方法
        stopwatch.Restart();
        var categoriesOptimized = await _advancedRepository.GetCategoriesWithProductsOptimizedAsync();
        stopwatch.Stop();
        var optimizedTime = stopwatch.ElapsedMilliseconds;

        // Assert - 驗證結果正確性和效能差異
        categoriesWithProblem.Should().HaveCount(3, "有問題的方法也要回傳正確的資料數量");
        categoriesOptimized.Should().HaveCount(3, "最佳化方法要回傳正確的資料數量");

        // 最佳化方法包含完整的關聯資料
        foreach (var category in categoriesOptimized)
        {
            category.Products.Should().NotBeEmpty("最佳化方法應該預載入產品資料");
        }

        // 記錄效能差異
        _testOutputHelper.WriteLine($"有問題的方法: {problemTime}ms");
        _testOutputHelper.WriteLine($"最佳化方法: {optimizedTime}ms");
    }

    /// <summary>
    /// 建立測試用的分類和產品資料
    /// </summary>
    private async Task CreateCategoriesWithProductsAsync()
    {
        for (var i = 1; i <= 3; i++)
        {
            var category = new Category
            {
                Name = $"N+1測試分類{i}",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            // 每個分類加入3個產品
            for (var j = 1; j <= 3; j++)
            {
                var product = new Product
                {
                    Name = $"N+1測試產品{i}-{j}",
                    Price = 100 + (i * 10) + j,
                    CategoryId = category.Id,
                    SKU = $"N1-{i}-{j}",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                _dbContext.Products.Add(product);
            }
        }

        await _dbContext.SaveChangesAsync();

        // 清除 DbContext 追蹤狀態，確保後續查詢會重新從資料庫載入
        _dbContext.ChangeTracker.Clear();
    }

    /// <summary>
    /// 清理測試資料並釋放 DbContext 資源。
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
    }
}