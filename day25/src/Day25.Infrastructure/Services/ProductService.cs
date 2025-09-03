using Day25.Infrastructure.Caching;
using Day25.Infrastructure.Data;

namespace Day25.Infrastructure.Services;

/// <summary>
/// 產品服務實作
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<ProductService> _logger;
    private readonly TimeProvider _timeProvider;

    private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(15);

    public ProductService(
        IProductRepository productRepository,
        ICacheService cacheService,
        ILogger<ProductService> logger,
        TimeProvider timeProvider)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 建立產品
    /// </summary>
    public async Task<ProductResponse> CreateAsync(ProductCreateRequest request, CancellationToken cancellationToken = default)
    {
        var now = _timeProvider.GetUtcNow();
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            CreatedAt = now,
            UpdatedAt = now
        };

        var createdProduct = await _productRepository.CreateAsync(product, cancellationToken);

        // 清除相關快取
        await _cacheService.RemoveByPatternAsync("products:query:*", cancellationToken);

        _logger.LogInformation("已建立產品 {ProductId}: {ProductName}", createdProduct.Id, createdProduct.Name);

        return MapToResponse(createdProduct);
    }

    /// <summary>
    /// 根據 ID 取得產品
    /// </summary>
    public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"products:single:{id}";

        // 嘗試從快取取得
        var cachedProduct = await _cacheService.GetAsync<ProductResponse>(cacheKey, cancellationToken);
        if (cachedProduct != null)
        {
            _logger.LogDebug("從快取取得產品 {ProductId}", id);
            return cachedProduct;
        }

        // 從資料庫取得
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return null;
        }

        var response = MapToResponse(product);

        // 設定快取
        await _cacheService.SetAsync(cacheKey, response, CacheExpiry, cancellationToken);

        return response;
    }

    /// <summary>
    /// 查詢產品列表
    /// </summary>
    public async Task<PagedResult<ProductResponse>> QueryAsync(
        string? keyword = null,
        int page = 1,
        int pageSize = 20,
        string sort = "createdAt",
        string direction = "desc",
        CancellationToken cancellationToken = default)
    {
        // 正規化排序欄位名稱
        var dbSortColumn = sort.ToLowerInvariant() switch
        {
            "name" => "name",
            "price" => "price",
            "createdat" => "created_at",
            "updatedat" => "updated_at",
            _ => "created_at"
        };

        var cacheKey = $"products:query:{keyword ?? "all"}:{page}:{pageSize}:{sort}:{direction}";

        // 嘗試從快取取得
        var cachedResult = await _cacheService.GetAsync<PagedResult<ProductResponse>>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            _logger.LogDebug("從快取取得產品查詢結果");
            return cachedResult;
        }

        // 從資料庫查詢
        var result = await _productRepository.QueryAsync(keyword, page, pageSize, dbSortColumn, direction, cancellationToken);

        var response = new PagedResult<ProductResponse>
        {
            Items = result.Items.Select(MapToResponse),
            Total = result.Total,
            Page = result.Page,
            PageSize = result.PageSize
        };

        // 設定快取 - 查詢結果快取時間較短
        await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(5), cancellationToken);

        return response;
    }

    /// <summary>
    /// 更新產品
    /// </summary>
    public async Task UpdateAsync(Guid id, ProductUpdateRequest request, CancellationToken cancellationToken = default)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (existingProduct == null)
        {
            throw new ProductNotFoundException(id);
        }

        existingProduct.Name = request.Name;
        existingProduct.Price = request.Price;
        existingProduct.UpdatedAt = _timeProvider.GetUtcNow();

        await _productRepository.UpdateAsync(existingProduct, cancellationToken);

        // 清除相關快取
        await _cacheService.RemoveAsync($"products:single:{id}", cancellationToken);
        await _cacheService.RemoveByPatternAsync("products:query:*", cancellationToken);

        _logger.LogInformation("已更新產品 {ProductId}: {ProductName}", id, request.Name);
    }

    /// <summary>
    /// 刪除產品
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var exists = await _productRepository.ExistsAsync(id, cancellationToken);
        if (!exists)
        {
            throw new ProductNotFoundException(id);
        }

        await _productRepository.DeleteAsync(id, cancellationToken);

        // 清除相關快取
        await _cacheService.RemoveAsync($"products:single:{id}", cancellationToken);
        await _cacheService.RemoveByPatternAsync("products:query:*", cancellationToken);

        _logger.LogInformation("已刪除產品 {ProductId}", id);
    }

    /// <summary>
    /// 將領域模型轉換為回應 DTO
    /// </summary>
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