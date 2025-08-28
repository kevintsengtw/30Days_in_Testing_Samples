using Day19.WebApplication.Entities;
using Day19.WebApplication.Models;

namespace Day19.WebApplication.Services;

/// <summary>
/// 貨運商服務介面
/// </summary>
public interface IShipperService
{
    /// <summary>
    /// 檢查貨運商是否存在
    /// </summary>
    /// <param name="shipperId">貨運商識別碼</param>
    Task<bool> IsExistsAsync(int shipperId);

    /// <summary>
    /// 取得貨運商
    /// </summary>
    /// <param name="shipperId">貨運商識別碼</param>
    Task<Shipper> GetAsync(int shipperId);

    /// <summary>
    /// 取得所有貨運商
    /// </summary>
    Task<IEnumerable<Shipper>> GetAllAsync();

    /// <summary>
    /// 建立貨運商
    /// </summary>
    /// <param name="parameter">建立參數</param>
    Task<Shipper> CreateAsync(ShipperCreateParameter parameter);

    /// <summary>
    /// 更新貨運商
    /// </summary>
    /// <param name="shipperId">貨運商識別碼</param>
    /// <param name="parameter">更新參數</param>
    Task<Shipper> UpdateAsync(int shipperId, ShipperCreateParameter parameter);

    /// <summary>
    /// 刪除貨運商
    /// </summary>
    /// <param name="shipperId">貨運商識別碼</param>
    Task DeleteAsync(int shipperId);
}