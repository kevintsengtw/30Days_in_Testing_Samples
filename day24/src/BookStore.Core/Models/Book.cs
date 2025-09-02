namespace BookStore.Core.Models;

/// <summary>
/// class Book
/// </summary>
/// <remarks>
/// 書籍實體類別 - 此類別代表書籍實體，包含書籍的基本資訊如標題、作者、價格及出版日期等。
/// </remarks>
public class Book
{
    /// <summary>
    /// 書籍唯一識別碼
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 書籍標題
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 作者姓名
    /// </summary>
    public string Author { get; set; } = string.Empty;

    /// <summary>
    /// 書籍價格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 出版日期
    /// </summary>
    public DateTime? PublishedDate { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? UpdatedDate { get; set; }
}