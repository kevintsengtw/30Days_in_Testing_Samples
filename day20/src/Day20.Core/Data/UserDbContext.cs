using Day20.Core.Models;

namespace Day20.Core.Data;

/// <summary>
/// SQL 資料庫 DbContext (支援 PostgreSQL 和 SQL Server)
/// </summary>
public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// 使用者資料集
    /// </summary>
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            // 設定主鍵
            entity.HasKey(e => e.Id);

            // 設定索引
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            // 設定屬性
            entity.Property(e => e.Id)
                  .HasMaxLength(36)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.Username)
                  .HasMaxLength(50)
                  .IsRequired();

            entity.Property(e => e.Email)
                  .HasMaxLength(100)
                  .IsRequired();

            entity.Property(e => e.FullName)
                  .HasMaxLength(100)
                  .IsRequired();

            // 根據資料庫提供者設定預設值
            if (Database.IsNpgsql())
            {
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("NOW()"); // PostgreSQL
            }
            else if (Database.IsSqlServer())
            {
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("GETUTCDATE()"); // SQL Server
            }
            else
            {
                // 其他資料庫或沒有預設值
                entity.Property(e => e.CreatedAt)
                      .HasDefaultValue(DateTime.UtcNow);
            }

            // 種子資料
            entity.HasData(
                new User
                {
                    Id = "1",
                    Username = "admin",
                    Email = "admin@example.com",
                    FullName = "系統管理員",
                    Age = 30,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = "2",
                    Username = "testuser",
                    Email = "test@example.com",
                    FullName = "測試使用者",
                    Age = 25,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            );
        });
    }
}