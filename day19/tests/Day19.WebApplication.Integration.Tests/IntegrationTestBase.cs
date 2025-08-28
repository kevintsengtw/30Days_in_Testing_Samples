using Day19.WebApplication.Data;
using Day19.WebApplication.Entities;
using Day19.WebApplication.Integration.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Day19.WebApplication.Integration.Tests;

/// <summary>
/// 整合測試基礎類別
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected IntegrationTestBase(CustomWebApplicationFactory factory)
    {
        this.Factory = factory;
        this.Client = factory.CreateClient();
    }

    /// <summary>
    /// 清理資料庫資料
    /// </summary>
    protected async Task CleanupDatabaseAsync()
    {
        using var scope = this.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // 確保資料庫已建立
        await context.Database.EnsureCreatedAsync();

        context.Shippers.RemoveRange(context.Shippers);
        await context.SaveChangesAsync();
    }

    /// <summary>
    /// 準備測試資料
    /// </summary>
    protected async Task<int> SeedShipperAsync(string companyName = "測試貨運公司", string phone = "02-1234-5678")
    {
        using var scope = this.Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // 確保資料庫已建立
        await context.Database.EnsureCreatedAsync();

        var shipper = new Shipper
        {
            CompanyName = companyName,
            Phone = phone
        };

        context.Shippers.Add(shipper);
        await context.SaveChangesAsync();

        return shipper.ShipperId;
    }
}