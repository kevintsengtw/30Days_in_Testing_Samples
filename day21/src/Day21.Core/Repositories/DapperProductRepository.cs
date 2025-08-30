using System.Text;
using Day21.Core.Models;
using Microsoft.Data.SqlClient;

namespace Day21.Core.Repositories;

/// <summary>
/// 使用 Dapper 實作的產品資料存取。
/// </summary>
public class DapperProductRepository(string connectionString) : IProductRepository, IProductByDapperRepository
{
    /// <summary>
    /// 取得所有產品。
    /// </summary>
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        await using var connection = new SqlConnection(connectionString);
        return await connection.QueryAsync<Product>("SELECT * FROM Products");
    }

    /// <summary>
    /// 依據 ID 取得特定產品。
    /// </summary>
    public async Task<Product?> GetByIdAsync(int id)
    {
        await using var connection = new SqlConnection(connectionString);
        return await connection.QuerySingleOrDefaultAsync<Product>("SELECT * FROM Products WHERE Id = @Id", new { Id = id });
    }

    /// <summary>
    /// 新增一個產品。
    /// </summary>
    public async Task AddAsync(Product product)
    {
        await using var connection = new SqlConnection(connectionString);
        var sql = """
                  INSERT INTO Products (Name, Description, Price, Stock, CategoryId, SKU, IsActive, CreatedAt) 
                  OUTPUT INSERTED.Id
                  VALUES (@Name, @Description, @Price, @Stock, @CategoryId, @SKU, @IsActive, @CreatedAt);
                  """;
        var id = await connection.QuerySingleAsync<int>(sql, product);
        product.Id = id;
    }

    /// <summary>
    /// 更新一個產品。
    /// </summary>
    public async Task UpdateAsync(Product product)
    {
        await using var connection = new SqlConnection(connectionString);
        var sql = """
                  UPDATE Products 
                  SET Name = @Name, 
                      Description = @Description,
                      Price = @Price, 
                      Stock = @Stock, 
                      CategoryId = @CategoryId, 
                      SKU = @SKU, 
                      IsActive = @IsActive
                  WHERE Id = @Id
                  """;
        await connection.ExecuteAsync(sql, product);
    }

    /// <summary>
    /// 刪除一個產品。
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        await using var connection = new SqlConnection(connectionString);
        var sql = "DELETE FROM Products WHERE Id = @Id";
        await connection.ExecuteAsync(sql, new { Id = id });
    }

    // IProductByDapperRepository 實作

    /// <summary>
    /// 使用 QueryMultiple 載入產品及其相關的標籤資料。
    /// </summary>
    public async Task<Product?> GetProductWithTagsAsync(int productId)
    {
        await using var connection = new SqlConnection(connectionString);
        var sql = """
                  SELECT * FROM Products WHERE Id = @ProductId;
                  SELECT t.* FROM Tags t
                  INNER JOIN ProductTags pt ON t.Id = pt.TagId
                  WHERE pt.ProductId = @ProductId;
                  """;

        using var multi = await connection.QueryMultipleAsync(sql, new { ProductId = productId });

        var product = await multi.ReadSingleOrDefaultAsync<Product>();
        if (product != null)
        {
            var tags = await multi.ReadAsync<Tag>();
            product.ProductTags = tags.Select(t => new ProductTag
            {
                ProductId = product.Id,
                TagId = t.Id,
                Tag = t
            }).ToList();
        }

        return product;
    }

    /// <summary>
    /// 使用 DynamicParameters 進行動態條件查詢。
    /// </summary>
    public async Task<IEnumerable<Product>> SearchProductsAsync(int? categoryId = null, decimal? minPrice = null, bool? isActive = null)
    {
        await using var connection = new SqlConnection(connectionString);

        var parameters = new DynamicParameters();
        var sqlBuilder = new StringBuilder("SELECT * FROM Products WHERE 1=1");

        if (categoryId.HasValue)
        {
            sqlBuilder.Append(" AND CategoryId = @CategoryId");
            parameters.Add("CategoryId", categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            sqlBuilder.Append(" AND Price >= @MinPrice");
            parameters.Add("MinPrice", minPrice.Value);
        }

        if (isActive.HasValue)
        {
            sqlBuilder.Append(" AND IsActive = @IsActive");
            parameters.Add("IsActive", isActive.Value);
        }

        return await connection.QueryAsync<Product>(sqlBuilder.ToString(), parameters);
    }

    /// <summary>
    /// 呼叫預存程序來產生產品銷售報表。
    /// </summary>
    public async Task<IEnumerable<ProductSalesReport>> GetProductSalesReportAsync(decimal minPrice)
    {
        await using var connection = new SqlConnection(connectionString);
        return await connection.QueryAsync<ProductSalesReport>(
            "sp_GetProductSalesReport",
            new { MinPrice = minPrice },
            commandType: CommandType.StoredProcedure
        );
    }
}