using Day21.Core.Repositories;
using Day21.Core.Tests.Infrastructure;

namespace Day21.Core.Tests;

/// <summary>
/// Dapper 進階功能的整合測試。
/// </summary>
[Collection(nameof(SqlServerCollectionFixture))]
public class DapperAdvancedTests : IDisposable
{
    private readonly IDbConnection _connection;
    private readonly IProductByDapperRepository _advancedRepository;
    private readonly IProductRepository _basicRepository;
    private readonly string _connectionString;
    private readonly ITestOutputHelper _testOutputHelper;

    public DapperAdvancedTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _connectionString = SqlServerContainerFixture.ConnectionString;
        _connection = new SqlConnection(_connectionString);
        _connection.Open();

        // 注入 Dapper 的 Repository 實作
        _advancedRepository = new DapperProductRepository(_connectionString);
        _basicRepository = new DapperProductRepository(_connectionString);

        // 確保測試資料庫物件存在
        EnsureDatabaseObjectsExist();
    }

    [Fact]
    public async Task GetProductWithTagsAsync_使用QueryMultiple_應該正確組合資料()
    {
        // Arrange
        var categoryId = await CreateTestCategoryAsync("QueryMultiple 分類");
        var product = await CreateAndAddTestProductAsync("多查詢商品", "MULTI-001", 100, categoryId, true);

        // 使用 Dapper 建立 Tag 和關聯
        var tagId1 = await CreateTestTagAsync("標籤A");
        var tagId2 = await CreateTestTagAsync("標籤B");
        await LinkProductAndTagAsync(product.Id, tagId1);
        await LinkProductAndTagAsync(product.Id, tagId2);

        // Act
        var result = await _advancedRepository.GetProductWithTagsAsync(product.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(product.Id);
        result.Name.Should().Be("多查詢商品");
        result.ProductTags.Should().HaveCount(2);
        result.ProductTags.Should().AllSatisfy(pt => pt.Tag.Should().NotBeNull());
        result.ProductTags.Select(pt => pt.Tag.Name).Should().Contain(new[] { "標籤A", "標籤B" });
    }

    [Fact]
    public async Task SearchProductsAsync_使用動態條件查詢_應該返回符合條件的商品()
    {
        // Arrange
        var categoryId = await CreateTestCategoryAsync("動態查詢分類");
        await CreateAndAddTestProductAsync("動態商品A", "DYN-A", 800, categoryId, true);
        await CreateAndAddTestProductAsync("動態商品B", "DYN-B", 1200, categoryId, true);
        await CreateAndAddTestProductAsync("動態商品C", "DYN-C", 1500, categoryId, false);

        // Act - 測試多重條件查詢
        var results = await _advancedRepository.SearchProductsAsync(
            categoryId: categoryId,
            minPrice: 1000,
            isActive: true
        );

        // Assert
        results.Should().HaveCount(1);
        var product = results.First();
        product.Name.Should().Be("動態商品B");
        product.Price.Should().Be(1200);
        product.IsActive.Should().BeTrue();
        product.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public async Task SearchProductsAsync_使用部分條件_應該返回符合條件的商品()
    {
        // Arrange
        var categoryId = await CreateTestCategoryAsync("部分條件分類");
        await CreateAndAddTestProductAsync("部分條件商品A", "PARTIAL-A", 500, categoryId, true);
        await CreateAndAddTestProductAsync("部分條件商品B", "PARTIAL-B", 1500, categoryId, true);

        // Act - 只使用價格條件
        var results = await _advancedRepository.SearchProductsAsync(minPrice: 1000);

        // Assert
        results.Should().HaveCount(1);
        results.First().Name.Should().Be("部分條件商品B");
    }

    [Fact]
    public async Task GetProductSalesReportAsync_呼叫預存程序_應該返回正確的銷售報表()
    {
        // Arrange
        var categoryId = await CreateTestCategoryAsync("銷售報表分類");
        var customerId = await CreateTestCustomerAsync("測試客戶");

        // 建立產品
        var product1 = await CreateAndAddTestProductAsync("高價商品", "SALES-HIGH", 1500, categoryId, true);
        var product2 = await CreateAndAddTestProductAsync("低價商品", "SALES-LOW", 500, categoryId, true);

        // 建立訂單和訂單項目
        var orderId = await CreateTestOrderAsync(customerId);
        await CreateTestOrderItemAsync(orderId, product1.Id, 2, 1500); // 數量 2, 單價 1500
        await CreateTestOrderItemAsync(orderId, product2.Id, 5, 500);  // 數量 5, 單價 500

        // Act
        var report = await _advancedRepository.GetProductSalesReportAsync(1000m);

        // Assert
        report.Should().NotBeEmpty();
        var highPriceProductReport = report.FirstOrDefault(r => r.Name == "高價商品");
        highPriceProductReport.Should().NotBeNull();
        highPriceProductReport!.TotalQuantity.Should().Be(2);
        highPriceProductReport.TotalRevenue.Should().Be(3000m);
    }

    /// <summary>
    /// 清理測試資料庫中的資料，確保每次測試後資料庫狀態一致。
    /// </summary>
    public void Dispose()
    {
        _connection.Execute("DELETE FROM ProductTags");
        _connection.Execute("DELETE FROM OrderItems");
        _connection.Execute("DELETE FROM Orders");
        _connection.Execute("DELETE FROM Products");
        _connection.Execute("DELETE FROM Categories");
        _connection.Execute("DELETE FROM Tags");
        _connection.Execute("DELETE FROM Customers");
        _connection.Dispose();
    }

    /// <summary>
    /// 確保資料庫中的必要物件（表格、預存程序等）存在。
    /// </summary>
    private void EnsureDatabaseObjectsExist()
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

        // 建立預存程序
        var storedProceduresDirectory = Path.Combine(scriptDirectory, "StoredProcedures");
        if (Directory.Exists(storedProceduresDirectory))
        {
            var spScriptFiles = Directory.GetFiles(storedProceduresDirectory, "*.sql");
            foreach (var scriptFile in spScriptFiles)
            {
                var script = File.ReadAllText(scriptFile);
                _connection.Execute(script);
            }
        }
    }

    /// <summary>
    /// 建立測試用的分類，並回傳其 Id。
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private async Task<int> CreateTestCategoryAsync(string name)
    {
        var sql = """
                  INSERT INTO Categories (
                      Name,
                      IsActive,
                      CreatedAt
                  )
                  OUTPUT INSERTED.Id
                  VALUES (
                      @Name,
                      1,
                      GETUTCDATE()
                  )
                  """;
        return await _connection.QuerySingleAsync<int>(sql, new { Name = name });
    }

    /// <summary>
    /// 建立測試用的客戶，並回傳其 Id。
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private async Task<int> CreateTestCustomerAsync(string name)
    {
        var sql = """
                  INSERT INTO Customers (
                      Name,
                      Email,
                      IsActive,
                      CreatedAt
                  )
                  OUTPUT INSERTED.Id
                  VALUES (
                      @Name,
                      @Email,
                      1,
                      GETUTCDATE()
                  )
                  """;
        return await _connection.QuerySingleAsync<int>(sql, new { Name = name, Email = $"{name}@test.com" });
    }

    /// <summary>
    /// 建立並新增測試用的商品，回傳該商品實體。
    /// </summary>
    /// <param name="name">name</param>
    /// <param name="sku">sku</param>
    /// <param name="price">price</param>
    /// <param name="categoryId">categoryId</param>
    /// <param name="isActive">isActive</param>
    /// <returns></returns>
    private async Task<Product> CreateAndAddTestProductAsync(string name, string sku, decimal price, int categoryId, bool isActive)
    {
        var product = new Product
        {
            Name = name,
            Price = price,
            CategoryId = categoryId,
            SKU = sku,
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };
        await _basicRepository.AddAsync(product);
        return product;
    }

    /// <summary>
    /// 建立測試用的標籤，並回傳其 Id。
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private async Task<int> CreateTestTagAsync(string name)
    {
        var sql = """
                  INSERT INTO Tags (
                      Name,
                      IsActive,
                      CreatedAt
                  )
                  OUTPUT INSERTED.Id
                  VALUES (
                      @Name,
                      1,
                      GETUTCDATE()
                  )
                  """;
        return await _connection.QuerySingleAsync<int>(sql, new { Name = name });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="tagId"></param>
    private async Task LinkProductAndTagAsync(int productId, int tagId)
    {
        var sql = """
                  INSERT INTO ProductTags (
                      ProductId,
                      TagId
                  )
                  VALUES (
                      @ProductId,
                      @TagId
                  )
                  """;
        await _connection.ExecuteAsync(sql, new { ProductId = productId, TagId = tagId });
    }

    /// <summary>
    /// 建立測試用的訂單，並回傳其 Id。
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    private async Task<int> CreateTestOrderAsync(int customerId)
    {
        var sql = """
                  INSERT INTO Orders (
                      OrderNumber,
                      CustomerId,
                      CustomerName,
                      CustomerEmail,
                      OrderDate,
                      Status,
                      TotalAmount,
                      IsActive,
                      CreatedAt
                  )
                  OUTPUT INSERTED.Id
                  VALUES (
                      @OrderNumber,
                      @CustomerId,
                      @CustomerName,
                      @CustomerEmail,
                      GETUTCDATE(),
                      'Completed',
                      0,
                      1,
                      GETUTCDATE()
                  )
                  """;
        return await _connection.QuerySingleAsync<int>(sql, new
        {
            OrderNumber = $"ORD{DateTime.Now:yyyyMMddHHmmss}",
            CustomerId = customerId,
            CustomerName = "測試客戶",
            CustomerEmail = "test@example.com"
        });
    }

    /// <summary>
    /// 建立測試用的訂單項目。
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="productId"></param>
    /// <param name="quantity"></param>
    /// <param name="unitPrice"></param>
    private async Task CreateTestOrderItemAsync(int orderId, int productId, int quantity, decimal unitPrice)
    {
        var subtotal = quantity * unitPrice;
        var sql = """
                  INSERT INTO OrderItems (
                      OrderId,
                      ProductId,
                      Quantity,
                      UnitPrice,
                      Subtotal
                  )
                  VALUES (
                      @OrderId,
                      @ProductId,
                      @Quantity,
                      @UnitPrice,
                      @Subtotal
                  )
                  """;
        await _connection.ExecuteAsync(sql, new { OrderId = orderId, ProductId = productId, Quantity = quantity, UnitPrice = unitPrice, Subtotal = subtotal });
    }
}