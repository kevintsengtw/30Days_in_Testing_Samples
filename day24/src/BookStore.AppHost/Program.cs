using System;
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// 添加 SQL Server 容器和資料庫
var database = builder.AddSqlServer("sql")
                      .AddDatabase("bookstore-db");

// 建立並運行應用程式
builder.Build().Run();