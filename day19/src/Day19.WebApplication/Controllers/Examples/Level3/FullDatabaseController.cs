using Day19.WebApplication.Data;
using Day19.WebApplication.Models;
using Microsoft.AspNetCore.Mvc;

namespace Day19.WebApplication.Controllers.Examples.Level3;

/// <summary>
/// Level 3 整合測試控制器：完整的資料庫整合
/// 特色：直接與 Entity Framework 和資料庫互動
/// 測試重點：完整的 CRUD 操作、事務處理、資料一致性、實際資料庫行為
/// </summary>
[ApiController]
[Route("api/v3/[controller]")]
public class FullDatabaseController : ControllerBase
{
    private readonly ShippingContext _context;
    private readonly TimeProvider _timeProvider;

    public FullDatabaseController(ShippingContext context, TimeProvider timeProvider)
    {
        _context = context;
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 取得所有出貨記錄，支援分頁
    /// </summary>
    [HttpGet("shipments")]
    public async Task<IActionResult> GetShipments([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page <= 0 || pageSize <= 0 || pageSize > 100)
        {
            return BadRequest(new { Error = "Invalid page parameters" });
        }

        try
        {
            var totalCount = await _context.Shipments.CountAsync();
            var shipments = await _context.Shipments
                                          .Include(s => s.Recipient)
                                          .OrderByDescending(s => s.CreatedAt)
                                          .Skip((page - 1) * pageSize)
                                          .Take(pageSize)
                                          .ToListAsync();

            var result = new
            {
                Data = shipments,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Failed to retrieve shipments", Details = ex.Message });
        }
    }

    /// <summary>
    /// 取得特定出貨記錄
    /// </summary>
    [HttpGet("shipments/{shipmentId}")]
    public async Task<IActionResult> GetShipment(int shipmentId)
    {
        if (shipmentId <= 0)
        {
            return BadRequest(new { Error = "Shipment ID must be greater than 0" });
        }

        try
        {
            var shipment = await _context.Shipments
                                         .Include(s => s.Recipient)
                                         .FirstOrDefaultAsync(s => s.Id == shipmentId);

            if (shipment == null)
            {
                return NotFound(new { Error = $"Shipment {shipmentId} not found" });
            }

            return Ok(shipment);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Failed to retrieve shipment", Details = ex.Message });
        }
    }

    /// <summary>
    /// 建立新的出貨記錄
    /// </summary>
    [HttpPost("shipments")]
    public async Task<IActionResult> CreateShipment([FromBody] CreateShipmentRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.RecipientName) ||
            string.IsNullOrWhiteSpace(request.Address))
        {
            return BadRequest(new { Error = "Invalid shipment data" });
        }

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            // 檢查是否已存在相同收件者的相同地址
            var existingRecipient = await _context.Recipients.FirstOrDefaultAsync(r => r.Name == request.RecipientName &&
                                                                                       r.Address == request.Address);

            Recipient recipient;
            if (existingRecipient != null)
            {
                recipient = existingRecipient;
            }
            else
            {
                // 建立新收件者
                recipient = new Recipient
                {
                    Name = request.RecipientName,
                    Address = request.Address,
                    Phone = request.RecipientPhone,
                    CreatedAt = _timeProvider.GetUtcNow().DateTime
                };

                _context.Recipients.Add(recipient);
                await _context.SaveChangesAsync(); // 儲存以取得 ID
            }

            // 建立出貨記錄
            var shipment = new Shipment
            {
                TrackingNumber = GenerateTrackingNumber(),
                RecipientId = recipient.Id,
                Status = ShipmentStatus.Pending,
                Weight = request.Weight,
                Cost = CalculateShippingCost(request.Weight),
                CreatedAt = _timeProvider.GetUtcNow().DateTime
            };

            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            // 重新查詢以包含關聯資料
            var createdShipment = await _context.Shipments
                                                .Include(s => s.Recipient)
                                                .FirstAsync(s => s.Id == shipment.Id);

            return CreatedAtAction(
                nameof(GetShipment),
                new { shipmentId = shipment.Id },
                createdShipment);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { Error = "Failed to create shipment", Details = ex.Message });
        }
    }

    /// <summary>
    /// 更新出貨狀態
    /// </summary>
    [HttpPut("shipments/{shipmentId}/status")]
    public async Task<IActionResult> UpdateShipmentStatus(int shipmentId, [FromBody] UpdateStatusRequest request)
    {
        if (shipmentId <= 0)
        {
            return BadRequest(new { Error = "Shipment ID must be greater than 0" });
        }

        if (request == null || !Enum.IsDefined(typeof(ShipmentStatus), request.Status))
        {
            return BadRequest(new { Error = "Invalid status" });
        }

        try
        {
            var shipment = await _context.Shipments
                                         .Include(s => s.Recipient)
                                         .FirstOrDefaultAsync(s => s.Id == shipmentId);

            if (shipment == null)
            {
                return NotFound(new { Error = $"Shipment {shipmentId} not found" });
            }

            // 檢查狀態轉換的有效性
            if (!IsValidStatusTransition(shipment.Status, request.Status))
            {
                return BadRequest(new { Error = $"Invalid status transition from {shipment.Status} to {request.Status}" });
            }

            shipment.Status = request.Status;
            shipment.UpdatedAt = _timeProvider.GetUtcNow().DateTime;

            await _context.SaveChangesAsync();

            return Ok(shipment);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Failed to update shipment status", Details = ex.Message });
        }
    }

    /// <summary>
    /// 刪除出貨記錄
    /// </summary>
    [HttpDelete("shipments/{shipmentId}")]
    public async Task<IActionResult> DeleteShipment(int shipmentId)
    {
        if (shipmentId <= 0)
        {
            return BadRequest(new { Error = "Shipment ID must be greater than 0" });
        }

        try
        {
            var shipment = await _context.Shipments.FindAsync(shipmentId);

            if (shipment == null)
            {
                return NotFound(new { Error = $"Shipment {shipmentId} not found" });
            }

            // 只有待處理或已取消的出貨才能刪除
            if (shipment.Status is ShipmentStatus.Shipped or ShipmentStatus.Delivered)
            {
                return BadRequest(new { Error = "Cannot delete shipped or delivered shipments" });
            }

            _context.Shipments.Remove(shipment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Failed to delete shipment", Details = ex.Message });
        }
    }

    /// <summary>
    /// 取得收件者清單
    /// </summary>
    [HttpGet("recipients")]
    public async Task<IActionResult> GetRecipients([FromQuery] string? searchTerm = null)
    {
        try
        {
            var query = _context.Recipients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(r => r.Name.Contains(searchTerm) || r.Address.Contains(searchTerm));
            }

            var recipients = await query
                                   .OrderBy(r => r.Name)
                                   .ToListAsync();

            return Ok(recipients);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Failed to retrieve recipients", Details = ex.Message });
        }
    }

    /// <summary>
    /// 取得收件者的出貨記錄
    /// </summary>
    [HttpGet("recipients/{recipientId}/shipments")]
    public async Task<IActionResult> GetRecipientShipments(int recipientId)
    {
        if (recipientId <= 0)
        {
            return BadRequest(new { Error = "Recipient ID must be greater than 0" });
        }

        try
        {
            var recipient = await _context.Recipients.FindAsync(recipientId);
            if (recipient == null)
            {
                return NotFound(new { Error = $"Recipient {recipientId} not found" });
            }

            var shipments = await _context.Shipments
                                          .Where(s => s.RecipientId == recipientId)
                                          .OrderByDescending(s => s.CreatedAt)
                                          .ToListAsync();

            return Ok(new { Recipient = recipient, Shipments = shipments });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Error = "Failed to retrieve recipient shipments", Details = ex.Message });
        }
    }

    // 私有輔助方法
    private string GenerateTrackingNumber()
    {
        return $"TRK{_timeProvider.GetUtcNow():yyyyMMdd}{Random.Shared.Next(1000, 9999)}";
    }

    private static decimal CalculateShippingCost(decimal weight)
    {
        return weight switch
        {
            <= 1 => 50,
            <= 5 => 100,
            <= 10 => 200,
            _ => 200 + (weight - 10) * 15
        };
    }

    private static bool IsValidStatusTransition(ShipmentStatus currentStatus, ShipmentStatus newStatus)
    {
        return currentStatus switch
        {
            ShipmentStatus.Pending => newStatus is ShipmentStatus.Processing or ShipmentStatus.Cancelled,
            ShipmentStatus.Processing => newStatus is ShipmentStatus.Shipped or ShipmentStatus.Cancelled,
            ShipmentStatus.Shipped => newStatus is ShipmentStatus.Delivered,
            ShipmentStatus.Delivered => false, // 已送達不能再變更
            ShipmentStatus.Cancelled => false, // 已取消不能再變更
            _ => false
        };
    }
}

// 請求模型
public class CreateShipmentRequest
{
    public string RecipientName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? RecipientPhone { get; set; }
    public decimal Weight { get; set; }
}

public class UpdateStatusRequest
{
    public ShipmentStatus Status { get; set; }
}