using Microsoft.EntityFrameworkCore;

namespace Day03.Core.Data;

/// <summary>
/// class AppDbContext - 應用程式資料庫上下文，負責與資料庫進行互動。
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// AppDbContext 的建構函式，接受資料庫選項並傳遞給基底類別。
    /// </summary>
    /// <param name="options">The options for the database context.</param>
    /// <returns></returns>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// 設定模型的資料表對應關係。
    /// </summary>
    /// <param name="modelBuilder">The modelBuilder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Age).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");

            // 將 UserSettings 作為 owned entity
            entity.OwnsOne(e => e.Settings, settings =>
            {
                settings.Property(s => s.Theme).HasMaxLength(50);
                settings.Property(s => s.Language).HasMaxLength(10);
            });
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETDATE()");
        });
    }
}