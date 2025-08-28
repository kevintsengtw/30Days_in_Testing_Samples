using Microsoft.AspNetCore.Mvc;

namespace Day19.WebApplication.Controllers.Examples.Level1;

/// <summary>
/// Level 1 整合測試控制器：簡單 WebApi 測試
/// 特色：無資料庫依賴、無外部服務依賴，專注於 Web 層測試
/// 測試重點：路由、HTTP 動詞、模型綁定、狀態碼、回應格式
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class BasicApiController : ControllerBase
{
    private readonly TimeProvider _timeProvider;

    public BasicApiController(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 健康檢查端點
    /// </summary>
    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        var healthStatus = new
        {
            Status = "Healthy",
            Timestamp = _timeProvider.GetUtcNow().DateTime,
            Version = "1.0.0"
        };

        return Ok(healthStatus);
    }

    /// <summary>
    /// Echo 訊息處理
    /// </summary>
    [HttpPost("echo")]
    public IActionResult Echo([FromBody] EchoRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { Error = "Message is required" });
        }

        var response = new EchoResponse
        {
            OriginalMessage = request.Message,
            EchoMessage = $"Echo: {request.Message}",
            ProcessedAt = _timeProvider.GetUtcNow().DateTime
        };

        return Ok(response);
    }

    /// <summary>
    /// 簡單數學計算
    /// </summary>
    [HttpGet("calculate/add")]
    public IActionResult Add([FromQuery] int a, [FromQuery] int b)
    {
        var result = new CalculationResult
        {
            Operation = "Addition",
            Input1 = a,
            Input2 = b,
            Result = a + b,
            CalculatedAt = _timeProvider.GetUtcNow().DateTime
        };

        return Ok(result);
    }

    /// <summary>
    /// 參數驗證範例
    /// </summary>
    [HttpGet("validate/{id}")]
    public IActionResult ValidateId(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new { Error = "ID must be greater than 0" });
        }

        if (id > 1000)
        {
            return BadRequest(new { Error = "ID must be less than or equal to 1000" });
        }

        return Ok(new { Id = id, Status = "Valid", ValidatedAt = _timeProvider.GetUtcNow().DateTime });
    }
}

// 資料模型
public class EchoRequest
{
    public string Message { get; set; } = string.Empty;
}

public class EchoResponse
{
    public string OriginalMessage { get; set; } = string.Empty;
    public string EchoMessage { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
}

public class CalculationResult
{
    public string Operation { get; set; } = string.Empty;
    public int Input1 { get; set; }
    public int Input2 { get; set; }
    public int Result { get; set; }
    public DateTime CalculatedAt { get; set; }
}