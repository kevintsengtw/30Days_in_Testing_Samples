namespace Day25.Application;

/// <summary>
/// 應用層服務註冊擴充方法
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 註冊應用層服務
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 產品服務的實作在 Infrastructure 層，這裡只定義介面
        // 實際的註冊會在 Infrastructure 的 DependencyInjection 中進行
        return services;
    }
}