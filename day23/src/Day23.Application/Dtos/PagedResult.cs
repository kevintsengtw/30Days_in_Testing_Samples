namespace Day23.Application.Dtos;

/// <summary>
/// 分頁結果
/// </summary>
/// <typeparam name="T">資料類型</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// 資料清單
    /// </summary>
    public IReadOnlyList<T> Items { get; set; } = [];

    /// <summary>
    /// 總筆數
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 目前頁數
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 總頁數
    /// </summary>
    public int PageCount { get; set; }
}