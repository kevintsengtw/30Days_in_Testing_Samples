using System.Text.Json;
using Day23.Application.Abstractions;
using Day23.Application.Dtos;
using Day23.Domain;

namespace Day23.Application.Services;

/// <summary>
/// 產品服務實作
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ICacheService _cache;
    private readonly TimeProvider _timeProvider;

    public ProductService(
        IProductRepository repository,
        ICacheService cache,
        TimeProvider timeProvider)
    {
        _repository = repository;
        _cache = cache;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 建立產品
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductResponse> CreateAsync(ProductCreateRequest request, CancellationToken cancellationToken = default)
    {
        var now = _timeProvider.GetUtcNow();
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Price = request.Price,
            CreatedAt = now,
            UpdatedAt = now
        };

        var createdProduct = await _repository.CreateAsync(product, cancellationToken);

        // 清除列表快取
        await _cache.RemoveByPrefixAsync(CacheKeys.ProductListPrefix, cancellationToken);

        return MapToResponse(createdProduct);
    }

    /// <summary>
    /// 根據 ID 取得產品
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // 先檢查快取
        var cacheKey = CacheKeys.Product(id);
        var cachedValue = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedValue != null)
        {
            var cachedProduct = JsonSerializer.Deserialize<Product>(cachedValue);
            return cachedProduct != null ? MapToResponse(cachedProduct) : null;
        }

        // 快取未命中，查詢資料庫
        var product = await _repository.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return null;
        }

        // 寫入快取
        var serializedProduct = JsonSerializer.Serialize(product);
        await _cache.SetStringAsync(cacheKey, serializedProduct, TimeSpan.FromMinutes(30), cancellationToken);

        return MapToResponse(product);
    }

    /// <summary>
    /// 查詢產品列表
    /// </summary>
    /// <param name="keyword"></param>
    /// <param name="page"></param>
    /// <param name="pageSize"></param>
    /// <param name="sort"></param>
    /// <param name="direction"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PagedResult<ProductResponse>> QueryAsync(
        string? keyword = null,
        int page = 1,
        int pageSize = 20,
        string sort = "createdAt",
        string direction = "desc",
        CancellationToken cancellationToken = default)
    {
        // 參數驗證
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 20;
        }

        if (pageSize > 100)
        {
            pageSize = 100;
        }

        // 檢查快取
        var cacheKey = CacheKeys.ProductList(keyword, page, pageSize, sort, direction);
        var cachedValue = await _cache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedValue != null)
        {
            var cachedResult = JsonSerializer.Deserialize<PagedResult<ProductResponse>>(cachedValue);
            if (cachedResult != null)
            {
                return cachedResult;
            }
        }

        // 查詢資料庫
        var products = await _repository.QueryAsync(keyword, page, pageSize, sort, direction, cancellationToken);
        var total = await _repository.CountAsync(keyword, cancellationToken);

        var result = new PagedResult<ProductResponse>
        {
            Items = products.Select(MapToResponse).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize,
            PageCount = (int)Math.Ceiling((double)total / pageSize)
        };

        // 寫入快取
        var serializedResult = JsonSerializer.Serialize(result);
        await _cache.SetStringAsync(cacheKey, serializedResult, TimeSpan.FromMinutes(10), cancellationToken);

        return result;
    }

    /// <summary>
    /// 更新產品
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task UpdateAsync(Guid id, ProductUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing == null)
        {
            throw new KeyNotFoundException($"找不到 ID 為 {id} 的產品");
        }

        existing.Name = request.Name.Trim();
        existing.Price = request.Price;
        existing.UpdatedAt = _timeProvider.GetUtcNow();

        await _repository.UpdateAsync(existing, cancellationToken);

        // 清除相關快取
        await _cache.RemoveAsync(CacheKeys.Product(id), cancellationToken);
        await _cache.RemoveByPrefixAsync(CacheKeys.ProductListPrefix, cancellationToken);
    }

    /// <summary>
    /// 刪除產品
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var exists = await _repository.ExistsAsync(id, cancellationToken);
        if (!exists)
        {
            throw new KeyNotFoundException($"找不到 ID 為 {id} 的產品");
        }

        await _repository.DeleteAsync(id, cancellationToken);

        // 清除相關快取
        await _cache.RemoveAsync(CacheKeys.Product(id), cancellationToken);
        await _cache.RemoveByPrefixAsync(CacheKeys.ProductListPrefix, cancellationToken);
    }

    /// <summary>
    /// 映射 Product 到 ProductResponse
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
    private static ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }
}