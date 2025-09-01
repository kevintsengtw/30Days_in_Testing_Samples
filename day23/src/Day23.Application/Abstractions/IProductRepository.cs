using Day23.Domain;

namespace Day23.Application.Abstractions;

/// <summary>
/// 產品儲存庫介面
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// 建立產品
    /// </summary>
    Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根據 ID 取得產品
    /// </summary>
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查詢產品列表
    /// </summary>
    Task<IReadOnlyList<Product>> QueryAsync(
        string? keyword = null,
        int page = 1,
        int pageSize = 20,
        string sort = "created_at",
        string direction = "desc",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 計算產品總數
    /// </summary>
    Task<int> CountAsync(string? keyword = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新產品
    /// </summary>
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刪除產品
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 檢查產品是否存在
    /// </summary>
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}