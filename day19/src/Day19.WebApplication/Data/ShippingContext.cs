using Day19.WebApplication.Models;

namespace Day19.WebApplication.Data;

/// <summary>
/// Level 3 範例專用的資料庫上下文
/// 用於完整的資料庫整合測試
/// </summary>
public class ShippingContext : DbContext
{
    public ShippingContext(DbContextOptions<ShippingContext> options) : base(options)
    {
    }

    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<Recipient> Recipients => Set<Recipient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 設定 Shipment 實體
        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TrackingNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.Weight).HasPrecision(10, 2);
            entity.Property(e => e.Cost).HasPrecision(10, 2);

            // 設定與 Recipient 的關聯
            entity.HasOne(e => e.Recipient)
                  .WithMany()
                  .HasForeignKey(e => e.RecipientId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // 設定 Recipient 實體
        modelBuilder.Entity<Recipient>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });
    }
}