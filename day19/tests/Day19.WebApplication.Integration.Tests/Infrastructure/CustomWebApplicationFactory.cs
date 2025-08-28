using Day19.WebApplication.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Day19.WebApplication.Integration.Tests.Infrastructure;

/// <summary>
/// 自訂 WebApplicationFactory 用於整合測試
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName;

    public CustomWebApplicationFactory()
    {
        // 每個 Factory 實例使用唯一的資料庫名稱以確保測試隔離
        this._databaseName = $"TestDb_{Guid.NewGuid()}";
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // 移除原有的 DbContext 設定
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // 重新加入測試用的 InMemory 資料庫
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName: this._databaseName);
                options.EnableSensitiveDataLogging();
            });
        });
    }
}