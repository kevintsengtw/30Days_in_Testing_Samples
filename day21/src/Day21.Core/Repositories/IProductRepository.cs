using Day21.Core.Models;

namespace Day21.Core.Repositories;

/// <summary>
/// 定義產品相關的資料存取操作。
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// 取得所有產品。
    /// </summary>
    /// <returns>產品列表。</returns>
    Task<IEnumerable<Product>> GetAllAsync();

    /// <summary>
    /// 依據 ID 取得特定產品。
    /// </summary>
    /// <param name="id">產品 ID。</param>
    /// <returns>對應的產品。</returns>
    Task<Product?> GetByIdAsync(int id);

    /// <summary>
    /// 新增一個產品。
    /// </summary>
    /// <param name="product">要新增的產品。</param>
    Task AddAsync(Product product);

    /// <summary>
    /// 更新一個產品。
    /// </summary>
    /// <param name="product">要更新的產品。</param>
    Task UpdateAsync(Product product);

    /// <summary>
    /// 刪除一個產品。
    /// </summary>
    /// <param name="id">要刪除的產品 ID。</param>
    Task DeleteAsync(int id);
}