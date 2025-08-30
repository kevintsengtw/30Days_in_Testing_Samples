using Day21.Core.Data;
using Day21.Core.Models;

namespace Day21.Core.Repositories;

/// <summary>
/// 使用 Entity Framework Core 實作的產品資料存取。
/// </summary>
public class EfCoreProductRepository(ECommerceDbContext context) : IProductRepository, IProductByEFCoreRepository
{
    /// <summary>
    /// 取得所有產品。
    /// </summary>
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await context.Products.ToListAsync();
    }

    /// <summary>
    /// 依據 ID 取得特定產品。
    /// </summary>
    public async Task<Product?> GetByIdAsync(int id)
    {
        return await context.Products.FindAsync(id);
    }

    /// <summary>
    /// 新增一個產品。
    /// </summary>
    public async Task AddAsync(Product product)
    {
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 更新一個產品。
    /// </summary>
    public async Task UpdateAsync(Product product)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 刪除一個產品。
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        var product = await context.Products.FindAsync(id);
        if (product != null)
        {
            context.Products.Remove(product);
            await context.SaveChangesAsync();
        }
    }

    // IProductByEFCoreRepository 實作

    /// <summary>
    /// 使用 Include 載入產品及其相關的分類和標籤資料。
    /// </summary>
    public async Task<Product?> GetProductWithCategoryAndTagsAsync(int productId)
    {
        return await context.Products
                            .Include(p => p.Category)
                            .Include(p => p.ProductTags)
                            .ThenInclude(pt => pt.Tag)
                            .FirstOrDefaultAsync(p => p.Id == productId);
    }

    /// <summary>
    /// 使用 AsSplitQuery 避免笛卡兒積問題來查詢產品。
    /// </summary>
    public async Task<IEnumerable<Product>> GetProductsByCategoryWithSplitQueryAsync(int categoryId)
    {
        return await context.Products
                            .AsSplitQuery()
                            .Include(p => p.Category)
                            .Include(p => p.ProductTags)
                            .ThenInclude(pt => pt.Tag)
                            .Where(p => p.CategoryId == categoryId && p.IsActive)
                            .ToListAsync();
    }

    /// <summary>
    /// 使用 ExecuteUpdateAsync 批次更新指定分類下的產品價格。
    /// </summary>
    public async Task<int> BatchUpdateProductPricesAsync(int categoryId, decimal priceMultiplier)
    {
        return await context.Products
                            .Where(p => p.CategoryId == categoryId && p.IsActive)
                            .ExecuteUpdateAsync(updates => updates
                                                           .SetProperty(p => p.Price, p => p.Price * priceMultiplier)
                                                           .SetProperty(p => p.UpdatedAt, DateTime.UtcNow));
    }

    /// <summary>
    /// 使用 ExecuteDeleteAsync 批次刪除非活躍狀態的產品。
    /// </summary>
    public async Task<int> BatchDeleteInactiveProductsAsync(int categoryId)
    {
        return await context.Products
                            .IgnoreQueryFilters() // 忽略全域查詢篩選器以查詢非活躍產品
                            .Where(p => p.CategoryId == categoryId && !p.IsActive)
                            .ExecuteDeleteAsync();
    }

    /// <summary>
    /// 使用 AsNoTracking 進行唯讀查詢以提升效能。
    /// </summary>
    public async Task<IEnumerable<Product>> GetProductsWithNoTrackingAsync(decimal minPrice)
    {
        return await context.Products
                            .AsNoTracking()
                            .Where(p => p.Price >= minPrice && p.IsActive)
                            .ToListAsync();
    }

    /// <summary>
    /// 使用 Include 查詢分類及其所有產品的一對多關聯資料。
    /// </summary>
    public async Task<Category?> GetCategoryWithProductsAsync(int categoryId)
    {
        return await context.Categories
                            .Include(c => c.Products)
                            .FirstOrDefaultAsync(c => c.Id == categoryId);
    }

    /// <summary>
    /// 使用 Include 和 ThenInclude 查詢產品的多層關聯資料（分類和標籤）。
    /// </summary>
    public async Task<Product?> GetProductWithMultiLevelIncludesAsync(int productId)
    {
        return await context.Products
                            .Include(p => p.Category)
                            .Include(p => p.ProductTags)
                            .ThenInclude(pt => pt.Tag)
                            .FirstOrDefaultAsync(p => p.Id == productId);
    }

    /// <summary>
    /// 使用複雜的 ThenInclude 查詢訂單的所有層級關聯資料（訂單項目→產品→分類）。
    /// </summary>
    public async Task<Order?> GetOrderWithComplexIncludesAsync(int orderId)
    {
        return await context.Orders
                            .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.Category)
                            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    /// <summary>
    /// 使用 ExecuteUpdateAsync 批次調整指定分類下所有商品的價格（打折場景）。
    /// </summary>
    public async Task<int> BatchApplyDiscountAsync(int categoryId, decimal discountPercentage)
    {
        return await context.Products
                            .Where(p => p.CategoryId == categoryId)
                            .ExecuteUpdateAsync(updates => updates
                                                           .SetProperty(p => p.Price, p => p.Price * discountPercentage)
                                                           .SetProperty(p => p.UpdatedAt, DateTime.UtcNow));
    }

    /// <summary>
    /// 取得所有分類及其產品資料（會產生 N+1 查詢問題的錯誤做法）。
    /// 此方法故意不使用 Include，導致每個分類都會額外查詢一次產品資料。
    /// </summary>
    public async Task<IEnumerable<Category>> GetCategoriesWithN1ProblemAsync()
    {
        var categories = await context.Categories.ToListAsync();

        // 故意觸發 N+1 查詢：每個分類都會產生一次額外的資料庫查詢
        foreach (var category in categories)
        {
            // 存取 Products 屬性會觸發 lazy loading 或額外查詢
            _ = category.Products.Count();
        }

        return categories;
    }

    /// <summary>
    /// 取得所有分類及其產品資料（正確做法，使用 Include 最佳化）。
    /// 使用 Include 預載入產品資料，避免 N+1 查詢問題。
    /// </summary>
    public async Task<IEnumerable<Category>> GetCategoriesWithProductsOptimizedAsync()
    {
        return await context.Categories
                            .Include(c => c.Products)
                            .ToListAsync();
    }

    /// <summary>
    /// 根據分類 ID 取得該分類的所有產品（用於模擬 N+1 查詢的後續查詢）。
    /// </summary>
    public async Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId)
    {
        return await context.Products
                            .Where(p => p.CategoryId == categoryId)
                            .ToListAsync();
    }
}