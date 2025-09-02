namespace BookStore.Core.Repositories;

/// <summary>
/// 書籍資料存取介面
/// </summary>
public interface IBookRepository
{
    /// <summary>
    /// 取得所有書籍
    /// </summary>
    Task<IEnumerable<Book>> GetAllAsync();

    /// <summary>
    /// 根據 ID 取得書籍
    /// </summary>
    Task<Book?> GetByIdAsync(int id);

    /// <summary>
    /// 新增書籍
    /// </summary>
    Task<Book> AddAsync(Book book);

    /// <summary>
    /// 更新書籍
    /// </summary>
    Task UpdateAsync(Book book);

    /// <summary>
    /// 刪除書籍
    /// </summary>
    Task DeleteAsync(int id);

    /// <summary>
    /// 根據作者取得書籍
    /// </summary>
    Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author);

    /// <summary>
    /// 取得高價書籍
    /// </summary>
    Task<IEnumerable<Book>> GetExpensiveBooksAsync(decimal minPrice);
}