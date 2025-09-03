namespace Day25.Domain.Exceptions;

/// <summary>
/// 產品不存在異常
/// </summary>
public class ProductNotFoundException : Exception
{
    public Guid ProductId { get; }

    public ProductNotFoundException(Guid productId)
        : base($"找不到 ID 為 {productId} 的產品")
    {
        ProductId = productId;
    }

    public ProductNotFoundException(Guid productId, Exception innerException)
        : base($"找不到 ID 為 {productId} 的產品", innerException)
    {
        ProductId = productId;
    }
}