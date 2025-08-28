using Day19.WebApplication.Models;
using Day19.WebApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace Day19.WebApplication.Controllers;

/// <summary>
/// 貨運商控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ShippersController : ControllerBase
{
    private readonly IShipperService _shipperService;
    private readonly ILogger<ShippersController> _logger;

    public ShippersController(IShipperService shipperService, ILogger<ShippersController> logger)
    {
        _shipperService = shipperService;
        _logger = logger;
    }

    /// <summary>
    /// 取得所有貨運商
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<SuccessResultOutputModel<List<ShipperOutputModel>>>> GetAllShippers()
    {
        _logger.LogInformation("取得所有貨運商");

        var shippers = await _shipperService.GetAllAsync();
        var result = new SuccessResultOutputModel<List<ShipperOutputModel>>
        {
            Status = "Success",
            Data = shippers.Select(s => new ShipperOutputModel
            {
                ShipperId = s.ShipperId,
                CompanyName = s.CompanyName,
                Phone = s.Phone
            }).ToList()
        };

        return Ok(result);
    }

    /// <summary>
    /// 取得貨運商資料
    /// </summary>
    /// <param name="id">貨運商識別碼</param>
    [HttpGet("{id}")]
    public async Task<ActionResult<SuccessResultOutputModel<ShipperOutputModel>>> GetShipper(int id)
    {
        _logger.LogInformation("取得貨運商資料：{ShipperId}", id);

        var exists = await _shipperService.IsExistsAsync(id);
        if (!exists)
        {
            return NotFound();
        }

        var shipper = await _shipperService.GetAsync(id);
        var result = new SuccessResultOutputModel<ShipperOutputModel>
        {
            Status = "Success",
            Data = new ShipperOutputModel
            {
                ShipperId = shipper.ShipperId,
                CompanyName = shipper.CompanyName,
                Phone = shipper.Phone
            }
        };

        return Ok(result);
    }

    /// <summary>
    /// 建立新貨運商
    /// </summary>
    /// <param name="parameter">建立參數</param>
    [HttpPost]
    public async Task<ActionResult<SuccessResultOutputModel<ShipperOutputModel>>> CreateShipper(
        ShipperCreateParameter parameter)
    {
        _logger.LogInformation("建立新貨運商：{CompanyName}", parameter.CompanyName);

        var shipper = await _shipperService.CreateAsync(parameter);
        var result = new SuccessResultOutputModel<ShipperOutputModel>
        {
            Status = "Success",
            Data = new ShipperOutputModel
            {
                ShipperId = shipper.ShipperId,
                CompanyName = shipper.CompanyName,
                Phone = shipper.Phone
            }
        };

        return CreatedAtAction(nameof(GetShipper), new { id = shipper.ShipperId }, result);
    }

    /// <summary>
    /// 更新貨運商資料
    /// </summary>
    /// <param name="id">貨運商識別碼</param>
    /// <param name="parameter">更新參數</param>
    [HttpPut("{id}")]
    public async Task<ActionResult<SuccessResultOutputModel<ShipperOutputModel>>> UpdateShipper(
        int id, ShipperCreateParameter parameter)
    {
        _logger.LogInformation("更新貨運商資料：{ShipperId}", id);

        var exists = await _shipperService.IsExistsAsync(id);
        if (!exists)
        {
            return NotFound();
        }

        var shipper = await _shipperService.UpdateAsync(id, parameter);
        var result = new SuccessResultOutputModel<ShipperOutputModel>
        {
            Status = "Success",
            Data = new ShipperOutputModel
            {
                ShipperId = shipper.ShipperId,
                CompanyName = shipper.CompanyName,
                Phone = shipper.Phone
            }
        };

        return Ok(result);
    }

    /// <summary>
    /// 刪除貨運商
    /// </summary>
    /// <param name="id">貨運商識別碼</param>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteShipper(int id)
    {
        _logger.LogInformation("刪除貨運商：{ShipperId}", id);

        var exists = await _shipperService.IsExistsAsync(id);
        if (!exists)
        {
            return NotFound();
        }

        await _shipperService.DeleteAsync(id);
        return NoContent();
    }
}