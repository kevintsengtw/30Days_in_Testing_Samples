using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// 加入 PostgreSQL 資料庫 - 使用 Session 生命週期，測試結束後自動清理
var postgres = builder.AddPostgres("postgres")
                      .WithLifetime(ContainerLifetime.Session);

var postgresDb = postgres.AddDatabase("productdb");

// 加入 Redis 快取 - 使用 Session 生命週期，測試結束後自動清理
// 配置 Redis TLS：使用開發者 HTTPS 自簽憑證 (Aspire 13.1.0 新增 TLS 終止支持)
var redis = builder.AddRedis("redis")
                   .WithLifetime(ContainerLifetime.Session)
                   .WithHttpsDeveloperCertificate();

// 加入 API 服務 - 使用強型別專案參考
var apiProject = builder.AddProject<Day25_Api>("day25-api")
                        .WithReference(postgresDb)
                        .WithReference(redis);

builder.Build().Run();