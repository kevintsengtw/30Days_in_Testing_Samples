namespace Day25.Tests.Integration.Infrastructure;

/// <summary>
/// 測試輔助方法
/// </summary>
public static class TestHelpers
{
    /// <summary>
    /// 建立產品建立請求
    /// </summary>
    public static ProductCreateRequest CreateProductRequest(string name, decimal price)
    {
        return new ProductCreateRequest
        {
            Name = name,
            Price = price
        };
    }

    /// <summary>
    /// 批量建立測試產品
    /// </summary>
    public static async Task SeedProductsAsync(DatabaseManager databaseManager, int count)
    {
        var connectionString = await databaseManager.GetConnectionStringAsync();
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        for (var i = 1; i <= count; i++)
        {
            var sql = @"
                INSERT INTO products (id, name, price, created_at, updated_at)
                VALUES (@id, @name, @price, @createdAt, @updatedAt)";

            await using var command = new NpgsqlCommand(sql, connection);
            command.Parameters.AddWithValue("id", Guid.NewGuid());
            command.Parameters.AddWithValue("name", $"測試產品 {i}");
            command.Parameters.AddWithValue("price", 100.00m + i);
            command.Parameters.AddWithValue("createdAt", DateTimeOffset.UtcNow);
            command.Parameters.AddWithValue("updatedAt", DateTimeOffset.UtcNow);

            await command.ExecuteNonQueryAsync();
        }
    }

    /// <summary>
    /// 建立指定的測試產品
    /// </summary>
    public static async Task<Guid> SeedSpecificProductAsync(DatabaseManager databaseManager, string name, decimal price)
    {
        var connectionString = await databaseManager.GetConnectionStringAsync();
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var productId = Guid.NewGuid();
        var sql = @"
            INSERT INTO products (id, name, price, created_at, updated_at)
            VALUES (@id, @name, @price, @createdAt, @updatedAt)";

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", productId);
        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("price", price);
        command.Parameters.AddWithValue("createdAt", DateTimeOffset.UtcNow);
        command.Parameters.AddWithValue("updatedAt", DateTimeOffset.UtcNow);

        await command.ExecuteNonQueryAsync();
        return productId;
    }

    /// <summary>
    /// 取得連線字串 (DatabaseManager 無法直接公開，透過反射取得)
    /// </summary>
    internal static async Task<string> GetConnectionStringAsync(this DatabaseManager databaseManager)
    {
        return await databaseManager.GetConnectionStringAsync();
    }
}