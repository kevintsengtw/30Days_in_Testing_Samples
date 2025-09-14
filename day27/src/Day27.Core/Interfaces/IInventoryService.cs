namespace Day27.Core.Interfaces;

/// <summary>
/// 庫存服務介面
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// 檢查庫存可用性
    /// </summary>
    /// <param name="productId">產品識別碼</param>
    /// <param name="quantity">需求數量</param>
    /// <returns>是否有足夠庫存</returns>
    Task<bool> CheckAvailabilityAsync(int productId, int quantity);

    /// <summary>
    /// 保留庫存
    /// </summary>
    /// <param name="productId">產品識別碼</param>
    /// <param name="quantity">保留數量</param>
    /// <returns>是否保留成功</returns>
    Task<bool> ReserveStockAsync(int productId, int quantity);
}