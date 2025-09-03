namespace Day25.Infrastructure.Data;

/// <summary>
/// 產品資料庫存取實作 - 使用 Dapper
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(NpgsqlDataSource dataSource, ILogger<ProductRepository> logger)
    {
        _dataSource = dataSource;
        _logger = logger;
    }

    /// <summary>
    /// 建立產品
    /// </summary>
    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            INSERT INTO products (id, name, price, created_at, updated_at)
            VALUES (@Id, @Name, @Price, @CreatedAt, @UpdatedAt)
            RETURNING id, name, price, created_at, updated_at";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var result = await connection.QuerySingleAsync<Product>(sql, product);

        _logger.LogInformation("已建立產品 {ProductId}", product.Id);
        return result;
    }

    /// <summary>
    /// 根據 ID 取得產品
    /// </summary>
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            SELECT id, name, price, created_at, updated_at
            FROM products
            WHERE id = @id";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Product>(sql, new { id });
    }

    /// <summary>
    /// 查詢產品列表
    /// </summary>
    public async Task<PagedResult<Product>> QueryAsync(
        string? keyword = null,
        int page = 1,
        int pageSize = 20,
        string sort = "created_at",
        string direction = "desc",
        CancellationToken cancellationToken = default)
    {
        // 驗證參數
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var validSortColumns = new[] { "name", "price", "created_at", "updated_at" };
        if (!validSortColumns.Contains(sort.ToLowerInvariant()))
        {
            sort = "created_at";
        }

        direction = direction.ToLowerInvariant() == "asc" ? "asc" : "desc";

        // 建構查詢條件
        var whereClause = "";
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            whereClause = "WHERE name ILIKE @keyword";
            parameters.Add("keyword", $"%{keyword}%");
        }

        // 查詢總數
        var countSql = $"SELECT COUNT(*) FROM products {whereClause}";

        // 查詢資料
        var offset = (page - 1) * pageSize;
        var dataSql = $@"
            SELECT id, name, price, created_at, updated_at
            FROM products 
            {whereClause}
            ORDER BY {sort} {direction}
            LIMIT @pageSize OFFSET @offset";

        parameters.Add("pageSize", pageSize);
        parameters.Add("offset", offset);

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var total = await connection.QuerySingleAsync<int>(countSql, parameters);
        var items = await connection.QueryAsync<Product>(dataSql, parameters);

        return new PagedResult<Product>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// 更新產品
    /// </summary>
    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        const string sql = @"
            UPDATE products 
            SET name = @Name, price = @Price, updated_at = @UpdatedAt
            WHERE id = @Id";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var rowsAffected = await connection.ExecuteAsync(sql, product);

        if (rowsAffected == 0)
        {
            throw new ProductNotFoundException(product.Id);
        }

        _logger.LogInformation("已更新產品 {ProductId}", product.Id);
    }

    /// <summary>
    /// 刪除產品
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM products WHERE id = @id";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var rowsAffected = await connection.ExecuteAsync(sql, new { id });

        if (rowsAffected == 0)
        {
            throw new ProductNotFoundException(id);
        }

        _logger.LogInformation("已刪除產品 {ProductId}", id);
    }

    /// <summary>
    /// 檢查產品是否存在
    /// </summary>
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM products WHERE id = @id)";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        return await connection.QuerySingleAsync<bool>(sql, new { id });
    }
}