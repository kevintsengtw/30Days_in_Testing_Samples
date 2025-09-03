namespace Day25.Application.Models;

/// <summary>
/// 分頁結果
/// </summary>
/// <typeparam name="T">項目類型</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// 項目列表
    /// </summary>
    public IEnumerable<T> Items { get; set; } = [];

    /// <summary>
    /// 總項目數
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 當前頁碼
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// 每頁項目數
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int PageCount => (int)Math.Ceiling((double)Total / PageSize);

    /// <summary>
    /// 是否有上一頁
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// 是否有下一頁
    /// </summary>
    public bool HasNextPage => Page < PageCount;
}