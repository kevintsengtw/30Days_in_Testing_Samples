using Day10.Core.Dtos;
using Day10.Core.Models;

namespace Day10.Core.Services;

/// <summary>
/// 產品服務
/// </summary>
public class ProductService
{
    /// <summary>
    /// 建立產品
    /// </summary>
    public Product CreateProduct(ProductCreateRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return new Product
        {
            Id = Random.Shared.Next(1, 10000),
            Name = request.Name,
            Price = request.Price,
            Category = request.Category,
            InStock = true
        };
    }

    /// <summary>
    /// 更新產品
    /// </summary>
    public bool UpdateProduct(int id, ProductUpdateRequest request)
    {
        if (id <= 0 || request == null)
        {
            return false;
        }

        // 模擬更新邏輯
        return true;
    }
}