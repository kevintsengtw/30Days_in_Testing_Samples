using Day13.Core.Dto;
using Day13.Core.Entities;
using Day13.Core.Misc;
using Day13.Core.Repositories;
using Day13.Core.Validation;
using MapsterMapper;
using Throw;

namespace Day13.Core.Services;

/// <summary>
/// class ShipperService - 出貨商服務實作
/// </summary>
public class ShipperService : IShipperService
{
    private readonly IMapper _mapper;
    private readonly IShipperRepository _shipperRepository;

    /// <summary>
    /// 建構函式
    /// </summary>
    /// <param name="mapper">對應器</param>
    /// <param name="shipperRepository">出貨商資料庫</param>
    public ShipperService(IMapper mapper, IShipperRepository shipperRepository)
    {
        this._mapper = mapper;
        this._shipperRepository = shipperRepository;
    }

    /// <summary>
    /// 以 ShipperId 查詢資料是否存在
    /// </summary>
    /// <param name="shipperId">出貨商編號</param>
    /// <returns>是否存在</returns>
    public async Task<bool> IsExistsAsync(int shipperId)
    {
        shipperId.Throw().IfLessThanOrEqualTo(0);
        var exists = await this._shipperRepository.IsExistsAsync(shipperId);
        return exists;
    }

    /// <summary>
    /// 以 ShipperId 查詢出貨商資料
    /// </summary>
    /// <param name="shipperId">出貨商編號</param>
    /// <returns>出貨商資料</returns>
    public async Task<ShipperDto> GetAsync(int shipperId)
    {
        shipperId.Throw().IfLessThanOrEqualTo(0);

        var exists = await this._shipperRepository.IsExistsAsync(shipperId);
        if (!exists)
        {
            return null;
        }

        var model = await this._shipperRepository.GetAsync(shipperId);
        var shipper = this._mapper.Map<ShipperModel, ShipperDto>(model);
        return shipper;
    }

    /// <summary>
    /// 取得出貨商資料總數
    /// </summary>
    /// <returns>資料總數</returns>
    public async Task<int> GetTotalCountAsync()
    {
        var totalCount = await this._shipperRepository.GetTotalCountAsync();
        return totalCount;
    }

    /// <summary>
    /// 取得所有出貨商資料
    /// </summary>
    /// <returns>所有出貨商資料</returns>
    public async Task<IEnumerable<ShipperDto>> GetAllAsync()
    {
        var models = await this._shipperRepository.GetAllAsync();
        var shippers = this._mapper.Map<IEnumerable<ShipperDto>>(models);
        return shippers;
    }

    /// <summary>
    /// 分頁取得出貨商資料
    /// </summary>
    /// <param name="from">起始位置</param>
    /// <param name="size">每頁筆數</param>
    /// <returns>分頁出貨商資料</returns>
    public async Task<IEnumerable<ShipperDto>> GetCollectionAsync(int from, int size)
    {
        from.Throw().IfLessThanOrEqualTo(0);
        size.Throw().IfLessThanOrEqualTo(0);

        var totalCount = await this.GetTotalCountAsync();
        if (totalCount.Equals(0))
        {
            return [];
        }

        var models = await this._shipperRepository.GetCollectionAsync(from, size);
        var shippers = this._mapper.Map<IEnumerable<ShipperDto>>(models);
        return shippers;
    }

    /// <summary>
    /// 搜尋出貨商資料
    /// </summary>
    /// <param name="companyName">公司名稱</param>
    /// <param name="phone">電話號碼</param>
    /// <returns>符合條件的出貨商資料</returns>
    public async Task<IEnumerable<ShipperDto>> SearchAsync(string companyName, string phone)
    {
        if (string.IsNullOrWhiteSpace(companyName) && string.IsNullOrWhiteSpace(phone))
        {
            throw new ArgumentException("companyName 與 phone 不可都為空白");
        }

        var totalCount = await this.GetTotalCountAsync();
        if (totalCount.Equals(0))
        {
            return [];
        }

        var models = await this._shipperRepository.SearchAsync(companyName ?? string.Empty, phone ?? string.Empty);
        var shippers = this._mapper.Map<IEnumerable<ShipperDto>>(models);
        return shippers;
    }

    /// <summary>
    /// 新增
    /// </summary>
    /// <param name="shipper">The shipper.</param>
    /// <returns></returns>
    public async Task<IResult> CreateAsync(ShipperDto shipper)
    {
        ModelValidator.Validate(shipper, nameof(shipper));

        var model = this._mapper.Map<ShipperDto, ShipperModel>(shipper);
        var result = await this._shipperRepository.CreateAsync(model);
        return result;
    }

    /// <summary>
    /// 修改
    /// </summary>
    /// <param name="shipper">The shipper.</param>
    /// <returns></returns>
    public async Task<IResult> UpdateAsync(ShipperDto shipper)
    {
        ModelValidator.Validate(shipper, nameof(shipper));

        IResult result = new Result(false);

        var exists = await this._shipperRepository.IsExistsAsync(shipper.ShipperId);
        if (!exists)
        {
            result.Message = "shipper not exists";
            return result;
        }

        var model = this._mapper.Map<ShipperDto, ShipperModel>(shipper);
        result = await this._shipperRepository.UpdateAsync(model);
        return result;
    }

    /// <summary>
    /// 刪除
    /// </summary>
    /// <param name="shipperId">shipperId</param>
    /// <returns></returns>
    public async Task<IResult> DeleteAsync(int shipperId)
    {
        shipperId.Throw().IfLessThanOrEqualTo(0);

        IResult result = new Result(false);

        var exists = await this._shipperRepository.IsExistsAsync(shipperId);
        if (!exists)
        {
            result.Message = "shipper not exists";
            return result;
        }

        result = await this._shipperRepository.DeleteAsync(shipperId);
        return result;
    }
}