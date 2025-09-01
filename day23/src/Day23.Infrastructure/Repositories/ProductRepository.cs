using Day23.Application.Abstractions;
using Day23.Domain;
using Day23.Infrastructure.Database;

namespace Day23.Infrastructure.Repositories;

/// <summary>
/// 產品儲存庫實作
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly DbConnectionFactory _connectionFactory;

    public ProductRepository(DbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// 建立產品
    /// </summary>
    /// <param name="product"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        const string sql = """
                           INSERT INTO products (id, name, price, created_at, updated_at)
                           VALUES (@Id, @Name, @Price, @CreatedAt, @UpdatedAt)
                           RETURNING id, name, price, created_at AS CreatedAt, updated_at AS UpdatedAt
                           """;

        await using var connection = _connectionFactory.Create();
        var result = await connection.QuerySingleAsync<Product>(sql, product);
        return result;
    }

    /// <summary>
    /// 根據 ID 取得產品
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = """
                           SELECT id, name, price, created_at AS CreatedAt, updated_at AS UpdatedAt
                           FROM products 
                           WHERE id = @id
                           """;

        await using var connection = _connectionFactory.Create();
        var result = await connection.QuerySingleOrDefaultAsync<Product>(sql, new { id });
        return result;
    }

    /// <summary>
    /// 查詢產品列表，支援關鍵字搜尋、分頁及排序
    /// </summary>
    /// <param name="keyword"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="sort"></param>
    /// <param name="direction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<Product>> QueryAsync(
        string? keyword = null,
        int page = 1,
        int pageSize = 20,
        string sort = "created_at",
        string direction = "desc",
        CancellationToken cancellationToken = default)
    {
        var whereClause = string.IsNullOrWhiteSpace(keyword)
            ? ""
            : "WHERE name ILIKE @keyword";

        var sortColumn = sort.ToLowerInvariant() switch
        {
            "name" => "name",
            "price" => "price",
            "createdat" => "created_at",
            "updatedat" => "updated_at",
            _ => "created_at"
        };

        var sortDirection = direction.ToLowerInvariant() == "asc" ? "ASC" : "DESC";
        var offset = (page - 1) * pageSize;

        var sql = $"""
                   SELECT id, name, price, created_at AS CreatedAt, updated_at AS UpdatedAt
                   FROM products
                   {whereClause}
                   ORDER BY {sortColumn} {sortDirection}
                   LIMIT @pageSize OFFSET @offset
                   """;

        await using var connection = _connectionFactory.Create();
        var parameters = new
        {
            keyword = $"%{keyword}%",
            pageSize,
            offset
        };

        var results = await connection.QueryAsync<Product>(sql, parameters);
        return results.ToList();
    }

    /// <summary>
    /// 計算符合條件的產品數量
    /// </summary>
    /// <param name="keyword"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> CountAsync(string? keyword = null, CancellationToken cancellationToken = default)
    {
        var whereClause = string.IsNullOrWhiteSpace(keyword)
            ? ""
            : "WHERE name ILIKE @keyword";

        var sql = $"""
                   SELECT COUNT(*)
                   FROM products
                   {whereClause}
                   """;

        await using var connection = _connectionFactory.Create();
        var parameters = new { keyword = $"%{keyword}%" };
        var count = await connection.QuerySingleAsync<int>(sql, parameters);
        return count;
    }

    /// <summary>
    /// 更新產品
    /// </summary>
    /// <param name="product"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        const string sql = """
                           UPDATE products
                           SET name = @Name, price = @Price, updated_at = @UpdatedAt
                           WHERE id = @Id
                           """;

        await using var connection = _connectionFactory.Create();
        var affectedRows = await connection.ExecuteAsync(sql, product);

        if (affectedRows == 0)
        {
            throw new KeyNotFoundException($"找不到 ID 為 {product.Id} 的產品");
        }
    }

    /// <summary>
    /// 刪除產品
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM products WHERE id = @id";

        await using var connection = _connectionFactory.Create();
        var affectedRows = await connection.ExecuteAsync(sql, new { id });

        if (affectedRows == 0)
        {
            throw new KeyNotFoundException($"找不到 ID 為 {id} 的產品");
        }
    }

    /// <summary>
    /// 檢查產品是否存在
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(*) FROM products WHERE id = @id";

        await using var connection = _connectionFactory.Create();
        var count = await connection.QuerySingleAsync<int>(sql, new { id });
        return count > 0;
    }
}