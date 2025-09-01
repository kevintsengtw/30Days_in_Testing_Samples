namespace Day23.Integration.Tests.Infrastructure;

/// <summary>
/// 測試輔助工具
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// 序列化物件為 JSON
    /// </summary>
    public static string ToJson<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    /// <summary>
    /// 從 JSON 反序列化物件
    /// </summary>
    public static T? FromJson<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    /// <summary>
    /// 建立測試用的產品建立請求
    /// </summary>
    public static ProductCreateRequest CreateProductRequest(
        string name = "測試產品",
        decimal price = 100.00m)
    {
        return new ProductCreateRequest
        {
            Name = name,
            Price = price
        };
    }

    /// <summary>
    /// 建立測試用的產品更新請求
    /// </summary>
    public static ProductUpdateRequest CreateProductUpdateRequest(
        string name = "更新產品",
        decimal price = 200.00m)
    {
        return new ProductUpdateRequest
        {
            Name = name,
            Price = price
        };
    }

    /// <summary>
    /// 驗證產品回應
    /// </summary>
    public static void AssertProductResponse(
        ProductResponse response,
        string expectedName,
        decimal expectedPrice,
        Guid? expectedId = null)
    {
        response.Should().NotBeNull();
        response.Name.Should().Be(expectedName);
        response.Price.Should().Be(expectedPrice);
        response.CreatedAt.Should().BeAfter(DateTimeOffset.MinValue);
        response.UpdatedAt.Should().BeAfter(DateTimeOffset.MinValue);

        if (expectedId.HasValue)
        {
            response.Id.Should().Be(expectedId.Value);
        }
        else
        {
            response.Id.Should().NotBe(Guid.Empty);
        }
    }

    /// <summary>
    /// 驗證分頁結果
    /// </summary>
    public static void AssertPagedResult<T>(
        PagedResult<T> result,
        int expectedTotalCount,
        int expectedPageSize,
        int expectedCurrentPage)
    {
        result.Should().NotBeNull();
        result.Total.Should().Be(expectedTotalCount);
        result.PageSize.Should().Be(expectedPageSize);
        result.Page.Should().Be(expectedCurrentPage);
        result.PageCount.Should().Be((int)Math.Ceiling((double)expectedTotalCount / expectedPageSize));
        result.Items.Should().NotBeNull();
    }

    /// <summary>
    /// 批量種子產品資料
    /// </summary>
    public static async Task SeedProductsAsync(DatabaseManager dbManager, int count)
    {
        var tasks = new List<Task>();
        for (var i = 1; i <= count; i++)
        {
            tasks.Add(SeedSpecificProductAsync(dbManager, $"產品 {i:D2}", i * 10.0m));
        }

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// 種子特定產品資料
    /// </summary>
    public static async Task<Guid> SeedSpecificProductAsync(
        DatabaseManager dbManager,
        string name,
        decimal price)
    {
        var productId = Guid.NewGuid();
        var sql = @"
            INSERT INTO products (id, name, price, created_at, updated_at)
            VALUES (@Id, @Name, @Price, @CreatedAt, @UpdatedAt)";

        var parameters = new
        {
            Id = productId,
            Name = name,
            Price = price,
            CreatedAt = DateTimeOffset.UtcNow, // 使用 UTC 時間
            UpdatedAt = DateTimeOffset.UtcNow  // 使用 UTC 時間
        };

        await dbManager.ExecuteAsync(sql, parameters);
        return productId;
    }
}