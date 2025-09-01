namespace Day23.WebApi.Controllers;

/// <summary>
/// class HealthController
/// </summary>
/// <remarks>
/// 健康檢查
/// </remarks>
[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly TimeProvider _timeProvider;

    public HealthController(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// 健康檢查端點
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        var response = new
        {
            status = "ok",
            timestamp = _timeProvider.GetUtcNow(),
            version = "1.0.0"
        };

        return this.Ok(response);
    }
}