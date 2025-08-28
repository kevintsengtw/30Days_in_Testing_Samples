using Day19.WebApplication.Data;
using Day19.WebApplication.Entities;
using Day19.WebApplication.Models;

namespace Day19.WebApplication.Services;

/// <summary>
/// 貨運商服務實作
/// </summary>
public class ShipperService : IShipperService
{
    private readonly AppDbContext _context;

    public ShipperService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 檢查貨運商是否存在
    /// </summary>
    public async Task<bool> IsExistsAsync(int shipperId)
    {
        return await _context.Shippers.AnyAsync(s => s.ShipperId == shipperId);
    }

    /// <summary>
    /// 取得貨運商
    /// </summary>
    public async Task<Shipper> GetAsync(int shipperId)
    {
        var shipper = await _context.Shippers.FindAsync(shipperId);
        return shipper ?? throw new InvalidOperationException($"找不到識別碼為 {shipperId} 的貨運商");
    }

    /// <summary>
    /// 取得所有貨運商
    /// </summary>
    public async Task<IEnumerable<Shipper>> GetAllAsync()
    {
        return await _context.Shippers.ToListAsync();
    }

    /// <summary>
    /// 建立貨運商
    /// </summary>
    public async Task<Shipper> CreateAsync(ShipperCreateParameter parameter)
    {
        var shipper = new Shipper
        {
            CompanyName = parameter.CompanyName,
            Phone = parameter.Phone
        };

        _context.Shippers.Add(shipper);
        await _context.SaveChangesAsync();

        return shipper;
    }

    /// <summary>
    /// 更新貨運商
    /// </summary>
    public async Task<Shipper> UpdateAsync(int shipperId, ShipperCreateParameter parameter)
    {
        var shipper = await GetAsync(shipperId);

        shipper.CompanyName = parameter.CompanyName;
        shipper.Phone = parameter.Phone;

        await _context.SaveChangesAsync();
        return shipper;
    }

    /// <summary>
    /// 刪除貨運商
    /// </summary>
    public async Task DeleteAsync(int shipperId)
    {
        var shipper = await GetAsync(shipperId);
        _context.Shippers.Remove(shipper);
        await _context.SaveChangesAsync();
    }
}