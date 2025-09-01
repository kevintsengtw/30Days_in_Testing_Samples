using Day23.Application.Abstractions;
using Day23.Application.Services;
using Day23.Application.Validation;
using Day23.Infrastructure.Caching;
using Day23.Infrastructure.Database;
using Day23.Infrastructure.Repositories;
using Day23.WebApi.Middleware;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateValidator>();

// Add ProblemDetails
builder.Services.AddProblemDetails();

// Add Exception Handler
builder.Services.AddExceptionHandler<FluentValidationExceptionHandler>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Add TimeProvider
builder.Services.AddSingleton(TimeProvider.System);

// Add Infrastructure services
builder.Services.AddSingleton<DbConnectionFactory>();
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Add Application services
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use Exception Handler
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// 為測試專案提供程式進入點的存取
public partial class Program
{
}