using Day22.Core.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;

namespace Day22.Integration.Tests.Fixtures;

/// <summary>
/// 測試用的設定提供者
/// </summary>
public static class TestSettings
{
    /// <summary>
    /// 建立測試用的 MongoDB 設定
    /// </summary>
    public static IOptions<MongoDbSettings> CreateMongoDbSettings()
    {
        var settings = new MongoDbSettings
        {
            ConnectionString = "mongodb://localhost:27017",
            DatabaseName = "test_day22_db",
            UsersCollectionName = "test_users",
            ConnectionTimeoutSeconds = 30,
            ServerSelectionTimeoutSeconds = 30
        };

        return Options.Create(settings);
    }

    /// <summary>
    /// 建立測試用的 Redis 設定
    /// </summary>
    public static IOptions<RedisSettings> CreateRedisSettings()
    {
        var settings = new RedisSettings
        {
            ConnectionString = "localhost:6379",
            Database = 1, // 使用測試資料庫
            KeyPrefix = "test:",
            DefaultExpiryMinutes = 60
        };

        return Options.Create(settings);
    }

    /// <summary>
    /// 建立測試用的 Logger
    /// </summary>
    public static ILogger<T> CreateLogger<T>()
    {
        return Substitute.For<ILogger<T>>();
    }

    /// <summary>
    /// 建立測試用的 TimeProvider
    /// </summary>
    public static TimeProvider CreateTimeProvider()
    {
        return TimeProvider.System;
    }

    /// <summary>
    /// 建立可控制的假時間提供者
    /// </summary>
    public static FakeTimeProvider CreateFakeTimeProvider(DateTime? startTime = null)
    {
        var fakeTimeProvider = new FakeTimeProvider();
        if (startTime.HasValue)
        {
            fakeTimeProvider.SetUtcNow(startTime.Value);
        }
        else
        {
            fakeTimeProvider.SetUtcNow(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));
        }

        return fakeTimeProvider;
    }
}