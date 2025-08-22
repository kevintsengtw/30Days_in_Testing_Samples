using Day13.Core.Entities;
using Day13.Core.Misc;

namespace Day13.Core.Repositories;

/// <summary>
/// 出貨商資料存取介面
/// </summary>
public interface IShipperRepository
{
    /// <summary>
    /// 檢查指定的出貨商編號是否存在
    /// </summary>
    /// <param name="shipperId">出貨商編號</param>
    /// <returns>是否存在</returns>
    Task<bool> IsExistsAsync(int shipperId);

    /// <summary>
    /// 根據出貨商編號取得出貨商資料
    /// </summary>
    /// <param name="shipperId">出貨商編號</param>
    /// <returns>出貨商資料</returns>
    Task<ShipperModel> GetAsync(int shipperId);

    /// <summary>
    /// 取得所有出貨商資料
    /// </summary>
    /// <returns>所有出貨商資料</returns>
    Task<IEnumerable<ShipperModel>> GetAllAsync();

    /// <summary>
    /// 取得出貨商資料總數
    /// </summary>
    /// <returns>資料總數</returns>
    Task<int> GetTotalCountAsync();

    /// <summary>
    /// 分頁取得出貨商資料
    /// </summary>
    /// <param name="from">起始位置</param>
    /// <param name="size">每頁筆數</param>
    /// <returns>分頁出貨商資料</returns>
    Task<IEnumerable<ShipperModel>> GetCollectionAsync(int from, int size);

    /// <summary>
    /// 搜尋出貨商資料
    /// </summary>
    /// <param name="companyName">公司名稱</param>
    /// <param name="phone">電話號碼</param>
    /// <returns>符合條件的出貨商資料</returns>
    Task<IEnumerable<ShipperModel>> SearchAsync(string companyName, string phone);

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns></returns>
    Task<IResult> CreateAsync(ShipperModel model);

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="model">The mdoel.</param>
    /// <returns></returns>
    Task<IResult> UpdateAsync(ShipperModel model);

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="shipperId">shipperId</param>
    /// <returns></returns>
    Task<IResult> DeleteAsync(int shipperId);
}