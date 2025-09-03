using Day25.Infrastructure.Caching;
using Day25.Infrastructure.Data;
using Day25.Infrastructure.Services;

namespace Day25.Infrastructure;

/// <summary>
/// 基礎設施層服務註冊擴充方法
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 註冊基礎設施層服務
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // 註冊資料存取服務
        services.AddScoped<IProductRepository, ProductRepository>();

        // 註冊快取服務
        services.AddScoped<ICacheService, RedisCacheService>();

        // 註冊應用服務
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}