namespace BookStore.Core.Services;

/// <summary>
/// 書籍業務邏輯實作
/// </summary>
public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private const decimal MIN_RECOMMENDED_PRICE = 100m;
    private const decimal MAX_RECOMMENDED_PRICE = 800m;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
    }

    /// <summary>
    /// 建立新書籍
    /// </summary>
    /// <param name="title">title</param>
    /// <param name="author">author</param>
    /// <param name="price">price</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Book> CreateBookAsync(string title, string author, decimal price)
    {
        // 業務邏輯驗證
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("書籍標題不可為空", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(author))
        {
            throw new ArgumentException("作者不可為空", nameof(author));
        }

        if (price <= 0)
        {
            throw new ArgumentException("價格必須大於零", nameof(price));
        }

        if (price > 10000)
        {
            throw new ArgumentException("價格不可超過 10,000 元", nameof(price));
        }

        // 建立書籍實體
        var book = new Book
        {
            Title = title.Trim(),
            Author = author.Trim(),
            Price = price,
            PublishedDate = DateTime.UtcNow
        };

        return await _bookRepository.AddAsync(book);
    }

    /// <summary>
    /// 根據 ID 取得書籍
    /// </summary>
    /// <param name="id">id</param>
    /// <returns></returns>
    public async Task<Book?> GetBookAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("書籍 ID 必須大於零", nameof(id));
        }

        return await _bookRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// 取得所有書籍
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await _bookRepository.GetAllAsync();
    }

    /// <summary>
    /// 更新書籍價格
    /// </summary>
    /// <param name="id">id</param>
    /// <param name="newPrice">newPrice</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Book> UpdateBookPriceAsync(int id, decimal newPrice)
    {
        if (id <= 0)
        {
            throw new ArgumentException("書籍 ID 必須大於零", nameof(id));
        }

        if (newPrice <= 0)
        {
            throw new ArgumentException("價格必須大於零", nameof(newPrice));
        }

        if (newPrice > 10000)
        {
            throw new ArgumentException("價格不可超過 10,000 元", nameof(newPrice));
        }

        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            throw new InvalidOperationException($"找不到 ID 為 {id} 的書籍");
        }

        book.Price = newPrice;
        await _bookRepository.UpdateAsync(book);

        return book;
    }

    /// <summary>
    /// 刪除書籍
    /// </summary>
    /// <param name="id">id</param>
    /// <exception cref="ArgumentException"></exception>
    public async Task DeleteBookAsync(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("書籍 ID 必須大於零", nameof(id));
        }

        await _bookRepository.DeleteAsync(id);
    }

    /// <summary>
    /// 搜尋作者的書籍
    /// </summary>
    /// <param name="author">author</param>
    /// <returns></returns>
    public async Task<IEnumerable<Book>> SearchBooksByAuthorAsync(string author)
    {
        if (string.IsNullOrWhiteSpace(author))
        {
            return [];
        }

        // 業務邏輯：支援部分匹配搜尋
        return await _bookRepository.GetBooksByAuthorAsync(author.Trim());
    }

    /// <summary>
    /// 取得推薦書籍（價格在合理範圍內）
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Book>> GetRecommendedBooksAsync()
    {
        // 業務邏輯：推薦價格在合理範圍內的書籍
        var expensiveBooks = await _bookRepository.GetExpensiveBooksAsync(MIN_RECOMMENDED_PRICE);

        return expensiveBooks
               .Where(b => b.Price <= MAX_RECOMMENDED_PRICE)
               .Take(10)
               .ToList();
    }
}