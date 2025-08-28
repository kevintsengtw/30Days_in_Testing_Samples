using System.Text;
using Day19.WebApplication.Controllers.Examples.Level3;
using Day19.WebApplication.Data;
using Day19.WebApplication.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Day19.WebApplication.Integration.Tests.Examples.Level3;

/// <summary>
/// Level 3 完整資料庫整合測試
/// 使用 FullDatabaseWebApplicationFactory 進行實際資料庫操作測試
/// </summary>
public class FullDatabaseIntegrationTests : IClassFixture<FullDatabaseWebApplicationFactory>, IDisposable
{
    private readonly FullDatabaseWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public FullDatabaseIntegrationTests(FullDatabaseWebApplicationFactory factory, ITestOutputHelper output)
    {
        this._factory = factory;
        this._client = factory.CreateClient();
        this._output = output;
    }

    [Fact]
    public async Task CreateShipment_輸入有效資料_應建立出貨記錄並回傳正確結果()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var createRequest = new CreateShipmentRequest
        {
            RecipientName = "測試收件者",
            Address = "台北市信義區市府路1號",
            RecipientPhone = "02-1234-5678",
            Weight = 2.5m
        };

        var json = JsonSerializer.Serialize(createRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await this._client.PostAsync("/api/v3/FullDatabase/shipments", content);

        // Assert
        response.Should().Be201Created()
                .And
                .Satisfy<Shipment>(shipment =>
                {
                    shipment.Id.Should().BeGreaterThan(0);
                    shipment.TrackingNumber.Should().NotBeNullOrEmpty();
                    shipment.Status.Should().Be(ShipmentStatus.Pending);
                    shipment.Weight.Should().Be(2.5m);
                    shipment.Cost.Should().BeGreaterThan(0);
                    shipment.Recipient.Should().NotBeNull();
                    shipment.Recipient.Name.Should().Be("測試收件者");
                    shipment.Recipient.Address.Should().Be("台北市信義區市府路1號");
                });

        // 驗證資料庫中確實有新增記錄
        using var scope = this._factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ShippingContext>();
        var shipment = await context.Shipments
                                    .Include(s => s.Recipient)
                                    .FirstOrDefaultAsync(s => s.Recipient.Name == "測試收件者");
        shipment.Should().NotBeNull();
    }

    [Fact]
    public async Task GetShipment_當出貨記錄存在_應回傳正確資料()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var shipmentId = await this.SeedShipmentAsync();

        // Act
        var response = await this._client.GetAsync($"/api/v3/FullDatabase/shipments/{shipmentId}");

        // Assert
        response.Should().Be200Ok()
                .And
                .Satisfy<Shipment>(shipment =>
                {
                    shipment.Id.Should().Be(shipmentId);
                    shipment.Status.Should().Be(ShipmentStatus.Pending);
                    shipment.Recipient.Should().NotBeNull();
                });
    }

    [Fact]
    public async Task UpdateShipmentStatus_當出貨記錄存在_應更新狀態並回傳正確結果()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var shipmentId = await this.SeedShipmentAsync();

        var updateRequest = new UpdateStatusRequest
        {
            Status = ShipmentStatus.Processing
        };

        var json = JsonSerializer.Serialize(updateRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await this._client.PutAsync($"/api/v3/FullDatabase/shipments/{shipmentId}/status", content);

        // Assert
        response.Should().Be200Ok()
                .And
                .Satisfy<Shipment>(shipment =>
                {
                    shipment.Id.Should().Be(shipmentId);
                    shipment.Status.Should().Be(ShipmentStatus.Processing);
                    shipment.UpdatedAt.Should().NotBeNull();
                });

        // 驗證資料庫中的資料確實已更新
        using var scope = this._factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ShippingContext>();
        var shipment = await context.Shipments.FindAsync(shipmentId);
        shipment.Should().NotBeNull();
        shipment!.Status.Should().Be(ShipmentStatus.Processing);
    }

    [Fact]
    public async Task DeleteShipment_當出貨記錄存在且狀態允許_應刪除資料()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var shipmentId = await this.SeedShipmentAsync();

        // Act
        var response = await this._client.DeleteAsync($"/api/v3/FullDatabase/shipments/{shipmentId}");

        // Assert
        response.Should().Be204NoContent();

        // 驗證資料庫中的資料確實已刪除
        using var scope = this._factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ShippingContext>();
        var shipment = await context.Shipments.FindAsync(shipmentId);
        shipment.Should().BeNull();
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 5)]
    public async Task GetShipments_支援分頁查詢_應回傳正確分頁資料(int page, int pageSize)
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        await this.SeedMultipleShipmentsAsync(10);

        // Act
        var response = await this._client.GetAsync($"/api/v3/FullDatabase/shipments?page={page}&pageSize={pageSize}");

        // Assert
        var contentString = await response.Content.ReadAsStringAsync();
        response.Should().Be200Ok();

        var responseData = JsonSerializer.Deserialize<JsonElement>(contentString);

        // 驗證分頁資訊（屬性名稱是小寫）
        responseData.GetProperty("page").GetInt32().Should().Be(page);
        responseData.GetProperty("pageSize").GetInt32().Should().Be(pageSize);
        responseData.GetProperty("totalCount").GetInt32().Should().Be(10);

        // 驗證資料陣列
        var dataArray = responseData.GetProperty("data");
        dataArray.ValueKind.Should().Be(JsonValueKind.Array);

        var shipments = dataArray.EnumerateArray().ToList();
        (shipments.Count <= pageSize).Should().BeTrue();

        // 每個出貨記錄都應該包含收件者資訊
        foreach (var shipment in shipments)
        {
            shipment.TryGetProperty("recipient", out var recipientProperty).Should().BeTrue();
            recipientProperty.ValueKind.Should().NotBe(JsonValueKind.Null);
        }
    }

    [Fact]
    public async Task CreateShipment_輸入無效資料_應回傳錯誤()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var invalidRequest = new CreateShipmentRequest
        {
            RecipientName = "", // 空字串應該會產生錯誤
            Address = "台北市信義區市府路1號",
            Weight = -1 // 負重量應該會產生錯誤
        };

        var json = JsonSerializer.Serialize(invalidRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await this._client.PostAsync("/api/v3/FullDatabase/shipments", content);

        // Assert
        response.Should().Be400BadRequest();
    }

    [Fact]
    public async Task GetShipment_當出貨記錄不存在_應回傳404()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        var nonExistentId = 999;

        // Act
        var response = await this._client.GetAsync($"/api/v3/FullDatabase/shipments/{nonExistentId}");

        // Assert
        response.Should().Be404NotFound();
    }

    [Fact]
    public async Task GetRecipients_應回傳收件者清單()
    {
        // Arrange
        await this.CleanupDatabaseAsync();
        await this.SeedMultipleShipmentsAsync(2);

        // Act
        var response = await this._client.GetAsync("/api/v3/FullDatabase/recipients");

        // Assert
        response.Should().Be200Ok()
                .And
                .Satisfy<List<Recipient>>(recipients =>
                {
                    recipients.Should().NotBeNull();
                    recipients.Count.Should().BeGreaterThan(0);
                    recipients.Should().OnlyContain(r => !string.IsNullOrEmpty(r.Name));
                });
    }

    private async Task CleanupDatabaseAsync()
    {
        using var scope = this._factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ShippingContext>();

        context.Shipments.RemoveRange(context.Shipments);
        context.Recipients.RemoveRange(context.Recipients);
        await context.SaveChangesAsync();
    }

    private async Task<int> SeedShipmentAsync()
    {
        using var scope = this._factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ShippingContext>();

        var recipient = new Recipient
        {
            Name = "測試收件者",
            Address = "台北市信義區市府路1號",
            Phone = "02-1234-5678"
        };

        context.Recipients.Add(recipient);
        await context.SaveChangesAsync();

        var shipment = new Shipment
        {
            TrackingNumber = $"TEST{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(1000, 9999)}",
            RecipientId = recipient.Id,
            Status = ShipmentStatus.Pending,
            Weight = 2.5m,
            Cost = 100m
        };

        context.Shipments.Add(shipment);
        await context.SaveChangesAsync();

        return shipment.Id;
    }

    private async Task SeedMultipleShipmentsAsync(int count)
    {
        using var scope = this._factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ShippingContext>();

        for (var i = 1; i <= count; i++)
        {
            var recipient = new Recipient
            {
                Name = $"收件者{i}",
                Address = $"台北市信義區市府路{i}號",
                Phone = $"02-1234-567{i}"
            };

            context.Recipients.Add(recipient);
            await context.SaveChangesAsync();

            var shipment = new Shipment
            {
                TrackingNumber = $"TEST{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(1000, 9999)}",
                RecipientId = recipient.Id,
                Status = ShipmentStatus.Pending,
                Weight = i * 1.5m,
                Cost = i * 50m
            };

            context.Shipments.Add(shipment);
        }

        await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        this._client?.Dispose();
    }
}