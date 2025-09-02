namespace BookStore.Core.Data;

/// <summary>
/// 書店資料庫內容
/// </summary>
public class BookStoreDbContext : DbContext
{
    public BookStoreDbContext(DbContextOptions<BookStoreDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// 書籍資料集
    /// </summary>
    public DbSet<Book> Books { get; set; }

    /// <summary>
    /// 模型建立
    /// </summary>
    /// <param name="modelBuilder">The modelBuilder</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Author).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
        });
    }
}