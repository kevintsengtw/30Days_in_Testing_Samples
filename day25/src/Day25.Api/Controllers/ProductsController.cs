namespace Day25.Api.Controllers;

/// <summary>
/// 產品控制器
/// </summary>
[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// 查詢產品列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductResponse>>> GetProducts(
        [FromQuery] string? keyword = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sort = "createdAt",
        [FromQuery] string direction = "desc",
        CancellationToken cancellationToken = default)
    {
        var result = await _productService.QueryAsync(keyword, page, pageSize, sort, direction, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// 根據 ID 取得產品
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetProduct(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);

        if (product == null)
        {
            return Problem(
                title: "資源不存在",
                detail: $"找不到 ID 為 {id} 的產品",
                statusCode: StatusCodes.Status404NotFound,
                type: "https://httpstatuses.com/404");
        }

        return Ok(product);
    }

    /// <summary>
    /// 建立產品
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> CreateProduct(
        [FromBody] ProductCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = await _productService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetProduct),
            new { id = product.Id },
            product);
    }

    /// <summary>
    /// 更新產品
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(
        Guid id,
        [FromBody] ProductUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _productService.UpdateAsync(id, request, cancellationToken);
            return NoContent();
        }
        catch (ProductNotFoundException)
        {
            return Problem(
                title: "資源不存在",
                detail: $"找不到 ID 為 {id} 的產品",
                statusCode: StatusCodes.Status404NotFound,
                type: "https://httpstatuses.com/404");
        }
    }

    /// <summary>
    /// 刪除產品
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _productService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (ProductNotFoundException)
        {
            return Problem(
                title: "資源不存在",
                detail: $"找不到 ID 為 {id} 的產品",
                statusCode: StatusCodes.Status404NotFound,
                type: "https://httpstatuses.com/404");
        }
    }
}