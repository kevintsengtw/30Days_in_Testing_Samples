using Day19.WebApplication.Entities;

namespace Day19.WebApplication.Data;

/// <summary>
/// 應用程式資料庫上下文
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// 貨運商資料集
    /// </summary>
    public DbSet<Shipper> Shippers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Shipper>(entity =>
        {
            entity.HasKey(e => e.ShipperId);
            entity.Property(e => e.CompanyName)
                  .IsRequired()
                  .HasMaxLength(40);
            entity.Property(e => e.Phone)
                  .HasMaxLength(24);
        });
    }
}