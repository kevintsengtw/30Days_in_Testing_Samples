namespace BookStore.Tests.Models;

/// <summary>
/// 書籍統計摘要
/// </summary>
public class BookSummary
{
    /// <summary>
    /// 作者姓名
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// 書籍數量
    /// </summary>
    public int BookCount { get; set; }

    /// <summary>
    /// 平均價格
    /// </summary>
    public double AveragePrice { get; set; }

    /// <summary>
    /// 最高價格
    /// </summary>
    public decimal MaxPrice { get; set; }
}