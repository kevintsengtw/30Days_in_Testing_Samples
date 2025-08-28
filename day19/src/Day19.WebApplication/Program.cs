using Day19.WebApplication.Controllers.Examples.Level2;
using Day19.WebApplication.Data;
using Day19.WebApplication.Services;

var builder = WebApplication.CreateBuilder(args);

// 加入服務
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 加入 TimeProvider (遵循 Day16 最佳實踐)
builder.Services.AddSingleton(TimeProvider.System);

// 加入 Entity Framework (支援 Level 3)
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("ShippingDb"));

// 加入 ShippingContext for Level 3 examples
builder.Services.AddDbContext<ShippingContext>(options => options.UseInMemoryDatabase("ShippingTestDb"));

// 加入業務邏輯服務
builder.Services.AddScoped<IShipperService, ShipperService>();

// 加入 Level 2 範例服務 (可以使用實際實作或預設為 Stub)
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

// 設定 HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// 讓測試專案可以存取這個類別
namespace Day19.WebApplication
{
    public partial class Program
    {
    }
}