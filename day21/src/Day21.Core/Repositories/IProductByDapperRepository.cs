using Day21.Core.Models;

namespace Day21.Core.Repositories;

/// <summary>
/// 銷售報表資料模型。
/// </summary>
public class ProductSalesReport
{
    /// <summary>
    /// 產品名稱。
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 總銷售數量。
    /// </summary>
    public int TotalQuantity { get; set; }

    /// <summary>
    /// 總銷售金額。
    /// </summary>
    public decimal TotalRevenue { get; set; }
}

/// <summary>
/// 定義 Dapper 特有的進階資料存取操作。
/// </summary>
public interface IProductByDapperRepository
{
    /// <summary>
    /// 使用 QueryMultiple 載入產品及其相關的標籤資料。
    /// </summary>
    /// <param name="productId">產品 ID。</param>
    /// <returns>包含標籤資料的產品。</returns>
    Task<Product?> GetProductWithTagsAsync(int productId);

    /// <summary>
    /// 使用 DynamicParameters 進行動態條件查詢。
    /// </summary>
    /// <param name="categoryId">分類 ID（可選）。</param>
    /// <param name="minPrice">最低價格（可選）。</param>
    /// <param name="isActive">是否活躍（可選）。</param>
    /// <returns>符合條件的產品列表。</returns>
    Task<IEnumerable<Product>> SearchProductsAsync(int? categoryId = null, decimal? minPrice = null, bool? isActive = null);

    /// <summary>
    /// 呼叫預存程序來產生產品銷售報表。
    /// </summary>
    /// <param name="minPrice">最低價格篩選條件。</param>
    /// <returns>銷售報表資料。</returns>
    Task<IEnumerable<ProductSalesReport>> GetProductSalesReportAsync(decimal minPrice);
}