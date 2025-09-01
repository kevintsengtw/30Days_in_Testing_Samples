using Day23.Integration.Tests.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Day23.Integration.Tests.Controllers;

/// <summary>
/// 異常處理器測試
/// </summary>
[Collection(IntegrationTestCollection.Name)]
public class ExceptionHandlerTests : IntegrationTestBase
{
    public ExceptionHandlerTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task GetById_當產品不存在_應回傳404且包含ProblemDetails()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await HttpClient.GetAsync($"/Products/{nonExistentId}");

        // Assert
        response.Should().Be404NotFound()
                .And.Satisfy<ProblemDetails>(problem =>
                {
                    problem.Type.Should().Be("https://httpstatuses.com/404");
                    problem.Title.Should().Be("產品不存在");
                    problem.Status.Should().Be(404);
                    problem.Detail.Should().Contain($"找不到 ID 為 {nonExistentId} 的產品");
                });
    }

    [Fact]
    public async Task Update_當產品不存在_應回傳404且包含ProblemDetails()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var updateRequest = new
        {
            Name = "更新的產品名稱",
            Price = 150.00m
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/Products/{nonExistentId}", updateRequest);

        // Assert
        response.Should().Be404NotFound();

        var content = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<JsonElement>(content);

        // 檢查 ProblemDetails 結構
        problemDetails.GetProperty("type").GetString().Should().Be("https://httpstatuses.com/404");
        problemDetails.GetProperty("title").GetString().Should().Be("產品不存在");
        problemDetails.GetProperty("status").GetInt32().Should().Be(404);
        problemDetails.GetProperty("detail").GetString().Should().Contain($"找不到 ID 為 {nonExistentId} 的產品");
    }

    [Fact]
    public async Task Delete_當產品不存在_應回傳404且包含ProblemDetails()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await HttpClient.DeleteAsync($"/Products/{nonExistentId}");

        // Assert
        response.Should().Be404NotFound();

        var content = await response.Content.ReadAsStringAsync();
        var problemDetails = JsonSerializer.Deserialize<JsonElement>(content);

        // 檢查 ProblemDetails 結構
        problemDetails.GetProperty("type").GetString().Should().Be("https://httpstatuses.com/404");
        problemDetails.GetProperty("title").GetString().Should().Be("產品不存在");
        problemDetails.GetProperty("status").GetInt32().Should().Be(404);
        problemDetails.GetProperty("detail").GetString().Should().Contain($"找不到 ID 為 {nonExistentId} 的產品");
    }

    [Fact]
    public async Task GetById_當ID格式錯誤_應回傳404NotFound()
    {
        // Arrange
        var invalidId = "invalid-guid-format";

        // Act
        var response = await HttpClient.GetAsync($"/Products/{invalidId}");

        // Assert - 路由約束 {id:guid} 不匹配時會回傳 404
        response.Should().Be404NotFound();
    }
}