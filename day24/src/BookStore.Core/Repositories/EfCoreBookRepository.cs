namespace BookStore.Core.Repositories;

/// <summary>
/// Entity Framework Core 書籍資料存取實作
/// </summary>
public class EfCoreBookRepository : IBookRepository
{
    private readonly BookStoreDbContext _context;

    public EfCoreBookRepository(BookStoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 取得所有書籍
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        return await _context.Books
                             .OrderBy(b => b.Title)
                             .ToListAsync();
    }

    /// <summary>
    /// 根據 ID 取得書籍
    /// </summary>
    /// <param name="id">id</param>
    /// <returns></returns>
    public async Task<Book?> GetByIdAsync(int id)
    {
        return await _context.Books
                             .FirstOrDefaultAsync(b => b.Id == id);
    }

    /// <summary>
    /// 新增書籍
    /// </summary>
    /// <param name="book">book</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Book> AddAsync(Book book)
    {
        if (book == null)
        {
            throw new ArgumentNullException(nameof(book));
        }

        if (string.IsNullOrWhiteSpace(book.Title))
        {
            throw new ArgumentException("書籍標題不可為空", nameof(book));
        }

        if (string.IsNullOrWhiteSpace(book.Author))
        {
            throw new ArgumentException("作者不可為空", nameof(book));
        }

        if (book.Price < 0)
        {
            throw new ArgumentException("價格不可為負數", nameof(book));
        }

        // 設定建立時間
        book.CreatedDate = DateTime.UtcNow;

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return book;
    }

    /// <summary>
    /// 更新書籍
    /// </summary>
    /// <param name="book">book</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task UpdateAsync(Book book)
    {
        if (book == null)
        {
            throw new ArgumentNullException(nameof(book));
        }

        var existingBook = await GetByIdAsync(book.Id);
        if (existingBook == null)
        {
            throw new InvalidOperationException($"找不到 ID 為 {book.Id} 的書籍");
        }

        // 設定更新時間
        book.UpdatedDate = DateTime.UtcNow;

        _context.Entry(existingBook).CurrentValues.SetValues(book);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 刪除書籍
    /// </summary>
    /// <param name="id">id</param>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task DeleteAsync(int id)
    {
        var book = await GetByIdAsync(id);
        if (book == null)
        {
            throw new InvalidOperationException($"找不到 ID 為 {id} 的書籍");
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 根據作者取得書籍
    /// </summary>
    /// <param name="author">author</param>
    /// <returns></returns>
    public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author)
    {
        if (string.IsNullOrWhiteSpace(author))
        {
            return [];
        }

        return await _context.Books
                             .Where(b => b.Author.Contains(author))
                             .OrderBy(b => b.Title)
                             .ToListAsync();
    }

    /// <summary>
    /// 取得高價書籍
    /// </summary>
    /// <param name="minPrice">minPrice</param>
    /// <returns></returns>
    public async Task<IEnumerable<Book>> GetExpensiveBooksAsync(decimal minPrice)
    {
        return await _context.Books
                             .Where(b => b.Price >= minPrice)
                             .OrderByDescending(b => b.Price)
                             .ToListAsync();
    }
}