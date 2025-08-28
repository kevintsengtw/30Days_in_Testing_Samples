using Day19.WebApplication.Data;
using Day19.WebApplication.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Day19.WebApplication.Integration.Tests.Examples.Level3;

/// <summary>
/// Level 3 整合測試的自訂 WebApplicationFactory
/// 特色：使用真實的 In-Memory 資料庫進行完整整合測試
/// 測試重點：資料庫 CRUD 操作、事務處理、Entity Framework 行為
/// </summary>
public class FullDatabaseWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 移除原本的資料庫上下文
            var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ShippingContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            // 移除原本的 ShippingContext
            var contextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ShippingContext));
            if (contextDescriptor != null)
            {
                services.Remove(contextDescriptor);
            }

            // 註冊 In-Memory 資料庫
            services.AddDbContext<ShippingContext>(options =>
            {
                options.UseInMemoryDatabase(this._databaseName);
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                // 忽略 In-Memory 資料庫的交易警告
                options.ConfigureWarnings(warnings =>
                                              warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            // 降低日誌等級以減少測試輸出噪音
            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.SetMinimumLevel(LogLevel.Warning);
            });
        });
    }

    /// <summary>
    /// 初始化資料庫並插入測試資料
    /// </summary>
    public async Task SeedTestDataAsync()
    {
        using var scope = this.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ShippingContext>();

        // 確保資料庫已建立
        await context.Database.EnsureCreatedAsync();

        // 清理現有資料
        context.Shipments.RemoveRange(context.Shipments);
        context.Recipients.RemoveRange(context.Recipients);
        await context.SaveChangesAsync();

        // 插入測試資料
        await SeedRecipientsAsync(context);
        await SeedShipmentsAsync(context);

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 清理資料庫
    /// </summary>
    public async Task CleanupDatabaseAsync()
    {
        using var scope = this.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ShippingContext>();

        context.Shipments.RemoveRange(context.Shipments);
        context.Recipients.RemoveRange(context.Recipients);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 取得資料庫上下文實例（用於測試中的直接資料操作）
    /// </summary>
    public ShippingContext GetDbContext()
    {
        var scope = this.Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<ShippingContext>();
    }

    private static async Task SeedRecipientsAsync(ShippingContext context)
    {
        var recipients = new[]
        {
            new Recipient
            {
                Id = 1,
                Name = "張小明",
                Address = "台北市信義區信義路五段7號",
                Phone = "0912345678",
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new Recipient
            {
                Id = 2,
                Name = "李小華",
                Address = "新北市板橋區文化路一段188號",
                Phone = "0987654321",
                CreatedAt = DateTime.UtcNow.AddDays(-25)
            },
            new Recipient
            {
                Id = 3,
                Name = "王大成",
                Address = "台中市西屯區台灣大道三段99號",
                Phone = "0923456789",
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            }
        };

        await context.Recipients.AddRangeAsync(recipients);
    }

    private static async Task SeedShipmentsAsync(ShippingContext context)
    {
        var shipments = new[]
        {
            new Shipment
            {
                Id = 1,
                TrackingNumber = "TRK202401011001",
                RecipientId = 1,
                Status = ShipmentStatus.Delivered,
                Weight = 2.5m,
                Cost = 100m,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new Shipment
            {
                Id = 2,
                TrackingNumber = "TRK202401011002",
                RecipientId = 2,
                Status = ShipmentStatus.Shipped,
                Weight = 1.2m,
                Cost = 50m,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-8)
            },
            new Shipment
            {
                Id = 3,
                TrackingNumber = "TRK202401011003",
                RecipientId = 1,
                Status = ShipmentStatus.Processing,
                Weight = 5.0m,
                Cost = 200m,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new Shipment
            {
                Id = 4,
                TrackingNumber = "TRK202401011004",
                RecipientId = 3,
                Status = ShipmentStatus.Pending,
                Weight = 0.8m,
                Cost = 50m,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        await context.Shipments.AddRangeAsync(shipments);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // 清理資源 - 對於 In-Memory 資料庫，不需要特別清理
            // 因為它們會在應用程式關閉時自動清理
        }

        base.Dispose(disposing);
    }
}