namespace Day08.Core.Interface;

/// <summary>
/// interface IInventoryService - 庫存服務介面
/// </summary>
public interface IInventoryService
{
    /// <summary>
    /// 檢查庫存是否足夠
    /// </summary>
    /// <param name="productId">商品編號</param>
    /// <param name="quantity">需求數量</param>
    /// <returns>是否有足夠庫存</returns>
    bool CheckStock(string productId, int quantity);
}