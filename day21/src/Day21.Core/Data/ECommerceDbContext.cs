using Day21.Core.Models;

namespace Day21.Core.Data;

/// <summary>
/// 電子商務資料庫上下文
/// </summary>
public class ECommerceDbContext : DbContext
{
    public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// 商品分類
    /// </summary>
    public DbSet<Category> Categories { get; set; }

    /// <summary>
    /// 商品
    /// </summary>
    public DbSet<Product> Products { get; set; }

    /// <summary>
    /// 商品標籤
    /// </summary>
    public DbSet<ProductTag> ProductTags { get; set; }

    /// <summary>
    /// 標籤
    /// </summary>
    public DbSet<Tag> Tags { get; set; }

    /// <summary>
    /// 客戶
    /// </summary>
    public DbSet<Customer> Customers { get; set; }

    /// <summary>
    /// 訂單
    /// </summary>
    public DbSet<Order> Orders { get; set; }

    /// <summary>
    /// 訂單項目
    /// </summary>
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 設定實體關聯
        ConfigureRelationships(modelBuilder);

        // 設定索引
        ConfigureIndexes(modelBuilder);

        // 設定查詢篩選器
        ConfigureQueryFilters(modelBuilder);
    }

    /// <summary>
    /// 設定實體關聯
    /// </summary>
    private static void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        // Product - Category (多對一)
        modelBuilder.Entity<Product>()
                    .HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

        // ProductTag - Product (多對一)
        modelBuilder.Entity<ProductTag>()
                    .HasKey(pt => new { pt.ProductId, pt.TagId });

        modelBuilder.Entity<ProductTag>()
                    .HasOne(pt => pt.Product)
                    .WithMany(p => p.ProductTags)
                    .HasForeignKey(pt => pt.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

        // ProductTag - Tag (多對一)
        modelBuilder.Entity<ProductTag>()
                    .HasOne(pt => pt.Tag)
                    .WithMany(t => t.ProductTags)
                    .HasForeignKey(pt => pt.TagId)
                    .OnDelete(DeleteBehavior.Cascade);

        // OrderItem - Order (多對一)
        modelBuilder.Entity<OrderItem>()
                    .HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

        // OrderItem - Product (多對一)
        modelBuilder.Entity<OrderItem>()
                    .HasOne(oi => oi.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

        // Order - Customer (多對一)
        modelBuilder.Entity<Order>()
                    .HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
    }

    /// <summary>
    /// 設定索引
    /// </summary>
    private static void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // Product 索引
        modelBuilder.Entity<Product>()
                    .HasIndex(p => p.SKU)
                    .IsUnique()
                    .HasFilter("[SKU] IS NOT NULL");

        modelBuilder.Entity<Product>()
                    .HasIndex(p => new { p.CategoryId, p.IsActive })
                    .HasDatabaseName("IX_Product_Category_Active");

        // Order 索引
        modelBuilder.Entity<Order>()
                    .HasIndex(o => o.OrderNumber)
                    .IsUnique();

        modelBuilder.Entity<Order>()
                    .HasIndex(o => o.CustomerEmail)
                    .HasDatabaseName("IX_Order_CustomerEmail");

        // ProductTag 已經有複合主鍵，不需要額外索引
        // 主鍵 (ProductId, TagId) 自動提供唯一性和索引效能
    }

    /// <summary>
    /// 設定查詢篩選器
    /// </summary>
    private static void ConfigureQueryFilters(ModelBuilder modelBuilder)
    {
        // 只查詢啟用的分類
        modelBuilder.Entity<Category>()
                    .HasQueryFilter(c => c.IsActive);

        // 只查詢啟用的商品
        modelBuilder.Entity<Product>()
                    .HasQueryFilter(p => p.IsActive);
    }
}