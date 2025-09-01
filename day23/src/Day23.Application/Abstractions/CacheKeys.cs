namespace Day23.Application.Abstractions;

/// <summary>
/// 快取鍵值管理
/// </summary>
public static class CacheKeys
{
    private const string Prefix = "day23:";

    /// <summary>
    /// 產品詳細資料快取鍵
    /// </summary>
    public static string Product(Guid id)
    {
        return $"{Prefix}products:{id}";
    }

    /// <summary>
    /// 產品列表快取鍵前綴
    /// </summary>
    public static string ProductListPrefix => $"{Prefix}products:list:";

    /// <summary>
    /// 產品列表快取鍵
    /// </summary>
    public static string ProductList(string? keyword, int page, int pageSize, string sort, string direction)
    {
        var keywordPart = string.IsNullOrWhiteSpace(keyword) ? "all" : keyword;
        return $"{ProductListPrefix}{keywordPart}:{page}:{pageSize}:{sort}:{direction}";
    }
}