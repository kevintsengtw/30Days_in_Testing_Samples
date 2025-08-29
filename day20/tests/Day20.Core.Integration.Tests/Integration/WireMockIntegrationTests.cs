using System.Text;
using Day20.Core.Services.Implementations;
using DotNet.Testcontainers.Containers;
using WireMock.Net.Testcontainers;
using Xunit.Abstractions;

namespace Day20.Core.Integration.Tests.Integration;

/// <summary>
/// WireMock 整合測試
/// </summary>
public class WireMockIntegrationTests : IAsyncLifetime
{
    private readonly WireMockContainer _wireMockContainer = new WireMockContainerBuilder().Build();

    private readonly ITestOutputHelper _output;
    private IExternalApiService _externalApiService = null!;
    private HttpClient _httpClient = null!;

    /// <summary>
    /// 建構式，注入 ITestOutputHelper 用於測試輸出
    /// </summary>
    /// <param name="output"></param>
    public WireMockIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    /// <summary>
    /// 測試初始化
    /// </summary>
    public async Task InitializeAsync()
    {
        await _wireMockContainer.StartAsync();

        _httpClient = new HttpClient();
        // WireMock 預設使用 port 80
        var baseUrl = $"http://localhost:{_wireMockContainer.GetMappedPublicPort(80)}";
        _externalApiService = new ExternalApiService(_httpClient, baseUrl, baseUrl);
    }

    /// <summary>
    /// 測試清理
    /// </summary>
    public async Task DisposeAsync()
    {
        _httpClient?.Dispose();
        await _wireMockContainer.DisposeAsync();
    }

    [Fact]
    public async Task ValidateEmailAsync_使用有效電子郵件_應回傳True()
    {
        // Arrange
        var email = "test@example.com";

        // 最簡單可行的 mapping - 回到工作的版本
        var mappingJson = """
                          {
                            "request": {
                              "method": "GET",
                              "urlPath": "/api/email/validate",
                              "queryParameters": {
                                "email": {
                                  "equalTo": "test@example.com"
                                }
                              }
                            },
                            "response": {
                              "status": 200,
                              "headers": {
                                "Content-Type": "application/json"
                              },
                              "body": "{\"IsValid\": true, \"Message\": \"Email is valid\"}"
                            }
                          }
                          """;

        // 使用 HttpClient 直接設定 WireMock mapping
        var adminUrl = $"http://localhost:{_wireMockContainer.GetMappedPublicPort(80)}/__admin/mappings";
        var content = new StringContent(mappingJson, Encoding.UTF8, "application/json");
        var mappingResponse = await _httpClient.PostAsync(adminUrl, content);
        mappingResponse.EnsureSuccessStatusCode();

        // 等待一點時間讓 mapping 生效
        await Task.Delay(100);

        // Act
        var result = await _externalApiService.ValidateEmailAsync(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidateEmailAsync_使用無效電子郵件_應回傳False()
    {
        // Arrange
        var email = "invalid-email";
        var mappingJson = """
                          {
                            "request": {
                              "method": "GET",
                              "urlPath": "/api/email/validate",
                              "queryParameters": {
                                "email": {
                                  "equalTo": "invalid-email"
                                }
                              }
                            },
                            "response": {
                              "status": 200,
                              "headers": {
                                "Content-Type": "application/json"
                              },
                              "body": "{\"IsValid\": false, \"Message\": \"Email is invalid\"}"
                            }
                          }
                          """;

        var adminUrl = $"http://localhost:{_wireMockContainer.GetMappedPublicPort(80)}/__admin/mappings";
        var content = new StringContent(mappingJson, Encoding.UTF8, "application/json");
        await _httpClient.PostAsync(adminUrl, content);

        // Act
        var result = await _externalApiService.ValidateEmailAsync(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateEmailAsync_API回傳錯誤_應回傳False()
    {
        // Arrange
        var email = "error@example.com";
        var mappingJson = """
                          {
                            "request": {
                              "method": "GET",
                              "urlPath": "/api/email/validate"
                            },
                            "response": {
                              "status": 500
                            }
                          }
                          """;

        var adminUrl = $"http://localhost:{_wireMockContainer.GetMappedPublicPort(80)}/__admin/mappings";
        var content = new StringContent(mappingJson, Encoding.UTF8, "application/json");
        await _httpClient.PostAsync(adminUrl, content);

        // Act
        var result = await _externalApiService.ValidateEmailAsync(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetLocationAsync_使用有效IP_應回傳位置資訊()
    {
        // Arrange
        var ipAddress = "8.8.8.8";
        var mappingJson = """
                          {
                            "request": {
                              "method": "GET",
                              "urlPath": "/api/location",
                              "queryParameters": {
                                "ip": {
                                  "equalTo": "8.8.8.8"
                                }
                              }
                            },
                            "response": {
                              "status": 200,
                              "headers": {
                                "Content-Type": "application/json"
                              },
                              "body": "{\"Country\": \"United States\", \"City\": \"Mountain View\", \"Region\": \"California\", \"Latitude\": 37.4056, \"Longitude\": -122.0775}"
                            }
                          }
                          """;

        var adminUrl = $"http://localhost:{_wireMockContainer.GetMappedPublicPort(80)}/__admin/mappings";
        var content = new StringContent(mappingJson, Encoding.UTF8, "application/json");
        await _httpClient.PostAsync(adminUrl, content);

        // Act
        var result = await _externalApiService.GetLocationAsync(ipAddress);

        // Assert
        result.Should().NotBeNull();
        result.Country.Should().Be("United States");
        result.City.Should().Be("Mountain View");
        result.Region.Should().Be("California");
        result.Latitude.Should().Be(37.4056);
        result.Longitude.Should().Be(-122.0775);
    }

    [Fact]
    public async Task GetLocationAsync_API回傳錯誤_應回傳預設值()
    {
        // Arrange
        var ipAddress = "192.168.1.1";
        var mappingJson = """
                          {
                            "request": {
                              "method": "GET",
                              "urlPath": "/api/location"
                            },
                            "response": {
                              "status": 404
                            }
                          }
                          """;

        var adminUrl = $"http://localhost:{_wireMockContainer.GetMappedPublicPort(80)}/__admin/mappings";
        var content = new StringContent(mappingJson, Encoding.UTF8, "application/json");
        await _httpClient.PostAsync(adminUrl, content);

        // Act
        var result = await _externalApiService.GetLocationAsync(ipAddress);

        // Assert
        result.Should().NotBeNull();
        result.Country.Should().Be("Unknown");
        result.City.Should().Be("Unknown");
        result.Region.Should().Be("Unknown");
        result.Latitude.Should().Be(0.0);
        result.Longitude.Should().Be(0.0);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task ValidateEmailAsync_使用空白電子郵件_應回傳False(string email)
    {
        // Act
        var result = await _externalApiService.ValidateEmailAsync(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ValidateEmailAsync_使用null電子郵件_應回傳False()
    {
        // Act
        var result = await _externalApiService.ValidateEmailAsync(null!);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetLocationAsync_使用空白IP_應拋出ArgumentException(string ipAddress)
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _externalApiService.GetLocationAsync(ipAddress));
    }

    [Fact]
    public async Task GetLocationAsync_使用nullIP_應拋出ArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _externalApiService.GetLocationAsync(null!));
    }

    [Fact]
    public async Task WireMockContainer_應該正確啟動並回應請求()
    {
        // Arrange
        var mappingJson = """
                          {
                            "request": {
                              "method": "GET",
                              "urlPath": "/test"
                            },
                            "response": {
                              "status": 200,
                              "body": "Hello from WireMock!"
                            }
                          }
                          """;

        var adminUrl = $"http://localhost:{_wireMockContainer.GetMappedPublicPort(80)}/__admin/mappings";
        var content = new StringContent(mappingJson, Encoding.UTF8, "application/json");
        await _httpClient.PostAsync(adminUrl, content);

        // Act
        var response = await _httpClient.GetAsync(
            $"http://localhost:{_wireMockContainer.GetMappedPublicPort(80)}/test");
        var responseContent = await response.Content.ReadAsStringAsync();

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        responseContent.Should().Be("Hello from WireMock!");

        // 驗證容器狀態
        _wireMockContainer.State.Should().Be(TestcontainersStates.Running);
    }
}