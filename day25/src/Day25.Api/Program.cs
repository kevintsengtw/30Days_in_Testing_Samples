using Day25.Api.Configuration;
using Day25.Api.ExceptionHandlers;
using Day25.Application;
using Day25.Infrastructure;
using Day25.Infrastructure.Data;
using Day25.Infrastructure.Validation;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// 初始化 Dapper 欄位映射規則 - 在其他服務註冊之前
DapperTypeMapping.Initialize();

// 加入 PostgreSQL - 直接使用 Aspire 整合
builder.AddNpgsqlDataSource("productdb");

// 加入 Redis - 直接使用 Aspire 整合
builder.AddRedisClient("redis");

// 加入 API 服務
builder.Services.AddControllers();

// 加入 API 探索
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 加入問題詳細資訊
builder.Services.AddProblemDetails();

// 加入異常處理器 - 順序很重要！
builder.Services.AddExceptionHandler<FluentValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// 加入應用服務
builder.Services.AddApplicationServices();

// 加入基礎設施服務
builder.Services.AddInfrastructureServices();

// 加入 FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateRequestValidator>();
builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

// 設定模型驗證
builder.Services.AddTransient<IValidationFilter, ValidationFilter>();
builder.Services.Configure<MvcOptions>(options => { options.Filters.Add<ValidationFilter>(); });

// 加入時間提供者
builder.Services.AddSingleton<TimeProvider>(TimeProvider.System);

var app = builder.Build();

// 配置 HTTP 請求管線
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 使用異常處理器
app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// 用於測試的 Program 類別
/// </summary>
public partial class Program
{
}