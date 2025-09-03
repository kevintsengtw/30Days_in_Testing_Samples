namespace Day25.Api.Controllers;

/// <summary>
/// 健康檢查控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// 健康檢查
    /// </summary>
    [HttpGet]
    public IActionResult GetHealth()
    {
        return Ok("Healthy");
    }

    /// <summary>
    /// 存活檢查
    /// </summary>
    [HttpGet("alive")]
    public IActionResult GetAlive()
    {
        return Ok("Healthy");
    }
}