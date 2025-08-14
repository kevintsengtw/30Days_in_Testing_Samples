using Day05.Domain.DomainModels;

namespace Day05.Domain.Services.BusinessServices;

/// <summary>
/// class ProductService - 產品服務
/// </summary>
public class ProductService
{
    /// <summary>
    /// 創建產品
    /// </summary>
    /// <param name="name">產品名稱</param>
    /// <param name="price">產品價格</param>
    /// <returns>創建的產品</returns>
    public Product Create(string name, decimal price)
    {
        return new Product
        {
            Id = Random.Shared.Next(1, 1000),
            Name = name,
            Price = price,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
    }
}