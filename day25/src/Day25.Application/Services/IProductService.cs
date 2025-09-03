using Day25.Application.DTOs;
using Day25.Application.Models;

namespace Day25.Application.Services;

/// <summary>
/// 產品服務介面
/// </summary>
public interface IProductService
{
    /// <summary>
    /// 建立產品
    /// </summary>
    Task<ProductResponse> CreateAsync(ProductCreateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根據 ID 取得產品
    /// </summary>
    Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 查詢產品列表
    /// </summary>
    Task<PagedResult<ProductResponse>> QueryAsync(
        string? keyword = null,
        int page = 1,
        int pageSize = 20,
        string sort = "createdAt",
        string direction = "desc",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新產品
    /// </summary>
    Task UpdateAsync(Guid id, ProductUpdateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刪除產品
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}