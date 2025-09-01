using Day23.Application.Dtos;
using Day23.Application.Services;

namespace Day23.WebApi.Controllers;

/// <summary>
/// class ProductsController
/// </summary>
/// <remarks>
/// 產品管理
/// </remarks>
[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// 建立產品
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Create(
        [FromBody] ProductCreateRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _productService.CreateAsync(request, cancellationToken);
        return this.CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// 根據 ID 取得產品
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _productService.GetByIdAsync(id, cancellationToken);

        if (result == null)
        {
            return this.NotFound(new ProblemDetails
            {
                Type = "https://httpstatuses.com/404",
                Title = "產品不存在",
                Status = 404,
                Detail = $"找不到 ID 為 {id} 的產品"
            });
        }

        return this.Ok(result);
    }

    /// <summary>
    /// 查詢產品列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductResponse>>> Query(
        [FromQuery] string? keyword = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sort = "createdAt",
        [FromQuery] string direction = "desc",
        CancellationToken cancellationToken = default)
    {
        // 參數驗證
        if (page < 1)
        {
            return this.BadRequest(new ProblemDetails
            {
                Type = "https://httpstatuses.com/400",
                Title = "無效的參數",
                Status = 400,
                Detail = "頁數必須大於 0"
            });
        }

        if (pageSize is < 1 or > 100)
        {
            return this.BadRequest(new ProblemDetails
            {
                Type = "https://httpstatuses.com/400",
                Title = "無效的參數",
                Status = 400,
                Detail = "每頁筆數必須在 1 到 100 之間"
            });
        }

        var validSortFields = new[] { "name", "price", "createdAt", "updatedAt" };
        if (!validSortFields.Contains(sort, StringComparer.OrdinalIgnoreCase))
        {
            return this.BadRequest(new ProblemDetails
            {
                Type = "https://httpstatuses.com/400",
                Title = "無效的參數",
                Status = 400,
                Detail = $"排序欄位必須為: {string.Join(", ", validSortFields)}"
            });
        }

        var validDirections = new[] { "asc", "desc" };
        if (!validDirections.Contains(direction, StringComparer.OrdinalIgnoreCase))
        {
            return this.BadRequest(new ProblemDetails
            {
                Type = "https://httpstatuses.com/400",
                Title = "無效的參數",
                Status = 400,
                Detail = "排序方向必須為 asc 或 desc"
            });
        }

        var result = await _productService.QueryAsync(keyword, page, pageSize, sort, direction, cancellationToken);
        return this.Ok(result);
    }

    /// <summary>
    /// 更新產品
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] ProductUpdateRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _productService.UpdateAsync(id, request, cancellationToken);
            return this.NoContent();
        }
        catch (KeyNotFoundException)
        {
            return this.NotFound(new ProblemDetails
            {
                Type = "https://httpstatuses.com/404",
                Title = "產品不存在",
                Status = 404,
                Detail = $"找不到 ID 為 {id} 的產品"
            });
        }
    }

    /// <summary>
    /// 刪除產品
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _productService.DeleteAsync(id, cancellationToken);
            return this.NoContent();
        }
        catch (KeyNotFoundException)
        {
            return this.NotFound(new ProblemDetails
            {
                Type = "https://httpstatuses.com/404",
                Title = "產品不存在",
                Status = 404,
                Detail = $"找不到 ID 為 {id} 的產品"
            });
        }
    }
}