using Day21.Core.Models;

namespace Day21.Core.Repositories;

/// <summary>
/// 定義 EF Core 特有的進階資料存取操作。
/// </summary>
public interface IProductByEFCoreRepository
{
    /// <summary>
    /// 使用 Include 載入產品及其相關的分類和標籤資料。
    /// </summary>
    /// <param name="productId">產品 ID。</param>
    /// <returns>包含完整關聯資料的產品。</returns>
    Task<Product?> GetProductWithCategoryAndTagsAsync(int productId);

    /// <summary>
    /// 使用 AsSplitQuery 避免笛卡兒積問題來查詢產品。
    /// </summary>
    /// <param name="categoryId">分類 ID。</param>
    /// <returns>該分類下的所有產品及其相關資料。</returns>
    Task<IEnumerable<Product>> GetProductsByCategoryWithSplitQueryAsync(int categoryId);

    /// <summary>
    /// 使用 ExecuteUpdateAsync 批次更新指定分類下的產品價格。
    /// </summary>
    /// <param name="categoryId">分類 ID。</param>
    /// <param name="priceMultiplier">價格倍數。</param>
    /// <returns>受影響的產品數量。</returns>
    Task<int> BatchUpdateProductPricesAsync(int categoryId, decimal priceMultiplier);

    /// <summary>
    /// 使用 ExecuteDeleteAsync 批次刪除非活躍狀態的產品。
    /// </summary>
    /// <param name="categoryId">分類 ID。</param>
    /// <returns>被刪除的產品數量。</returns>
    Task<int> BatchDeleteInactiveProductsAsync(int categoryId);

    /// <summary>
    /// 使用 AsNoTracking 進行唯讀查詢以提升效能。
    /// </summary>
    /// <param name="minPrice">最低價格。</param>
    /// <returns>價格大於等於指定值的產品列表。</returns>
    Task<IEnumerable<Product>> GetProductsWithNoTrackingAsync(decimal minPrice);

    /// <summary>
    /// 使用 Include 查詢分類及其所有產品的一對多關聯資料。
    /// </summary>
    /// <param name="categoryId">分類 ID。</param>
    /// <returns>包含所有產品的分類。</returns>
    Task<Category?> GetCategoryWithProductsAsync(int categoryId);

    /// <summary>
    /// 使用 Include 和 ThenInclude 查詢產品的多層關聯資料（分類和標籤）。
    /// </summary>
    /// <param name="productId">產品 ID。</param>
    /// <returns>包含分類和標籤的產品完整資料。</returns>
    Task<Product?> GetProductWithMultiLevelIncludesAsync(int productId);

    /// <summary>
    /// 使用複雜的 ThenInclude 查詢訂單的所有層級關聯資料（訂單項目→產品→分類）。
    /// </summary>
    /// <param name="orderId">訂單 ID。</param>
    /// <returns>包含完整關聯資料的訂單。</returns>
    Task<Order?> GetOrderWithComplexIncludesAsync(int orderId);

    /// <summary>
    /// 使用 ExecuteUpdateAsync 批次調整指定分類下所有商品的價格（打折場景）。
    /// </summary>
    /// <param name="categoryId">分類 ID。</param>
    /// <param name="discountPercentage">折扣比例（如 0.8 表示 8 折）。</param>
    /// <returns>受影響的商品數量。</returns>
    Task<int> BatchApplyDiscountAsync(int categoryId, decimal discountPercentage);

    /// <summary>
    /// 取得所有分類及其產品資料（會產生 N+1 查詢問題的錯誤做法）。
    /// 此方法故意不使用 Include，導致每個分類都會額外查詢一次產品資料。
    /// </summary>
    /// <returns>所有分類列表（包含產品資料，但會有效能問題）。</returns>
    Task<IEnumerable<Category>> GetCategoriesWithN1ProblemAsync();

    /// <summary>
    /// 取得所有分類及其產品資料（正確做法，使用 Include 最佳化）。
    /// 使用 Include 預載入產品資料，避免 N+1 查詢問題。
    /// </summary>
    /// <returns>所有分類列表（包含完整的產品關聯資料）。</returns>
    Task<IEnumerable<Category>> GetCategoriesWithProductsOptimizedAsync();

    /// <summary>
    /// 根據分類 ID 取得該分類的所有產品（用於模擬 N+1 查詢的後續查詢）。
    /// </summary>
    /// <param name="categoryId">分類 ID。</param>
    /// <returns>該分類下的所有產品。</returns>
    Task<IEnumerable<Product>> GetProductsByCategoryIdAsync(int categoryId);
}