using System.Text.Json;

namespace Day20.Core.Services.Implementations;

/// <summary>
/// 外部 API 服務實作
/// </summary>
public class ExternalApiService : IExternalApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _emailValidationApiUrl;
    private readonly string _locationApiUrl;

    /// <summary>
    /// 建構函式
    /// </summary>
    /// <param name="httpClient">HTTP 客戶端</param>
    /// <param name="emailValidationApiUrl">電子郵件驗證 API URL</param>
    /// <param name="locationApiUrl">地理位置 API URL</param>
    public ExternalApiService(
        HttpClient httpClient,
        string emailValidationApiUrl = "http://localhost:8080",
        string locationApiUrl = "http://localhost:8080")
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _emailValidationApiUrl = emailValidationApiUrl;
        _locationApiUrl = locationApiUrl;
    }

    /// <summary>
    /// 驗證使用者電子郵件地址
    /// </summary>
    public async Task<bool> ValidateEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            var response = await _httpClient.GetAsync(
                $"{_emailValidationApiUrl}/api/email/validate?email={Uri.EscapeDataString(email)}",
                cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonSerializer.Deserialize<EmailValidationResponse>(content);
                return result?.IsValid ?? false;
            }

            return false;
        }
        catch (Exception)
        {
            // 在實際環境中，這裡應該記錄錯誤
            return false;
        }
    }

    /// <summary>
    /// 取得使用者的地理位置資訊
    /// </summary>
    public async Task<LocationInfo> GetLocationAsync(string ipAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            throw new ArgumentException("IP 地址不能為空", nameof(ipAddress));
        }

        try
        {
            var response = await _httpClient.GetAsync(
                $"{_locationApiUrl}/api/location?ip={Uri.EscapeDataString(ipAddress)}",
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return new LocationInfo("Unknown", "Unknown", "Unknown", 0.0, 0.0);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            // 檢查是否為有效的 JSON
            if (string.IsNullOrWhiteSpace(content))
            {
                return new LocationInfo("Unknown", "Unknown", "Unknown", 0.0, 0.0);
            }

            var result = JsonSerializer.Deserialize<LocationResponse>(content);

            return new LocationInfo(
                result?.Country ?? "Unknown",
                result?.City ?? "Unknown",
                result?.Region ?? "Unknown",
                result?.Latitude ?? 0.0,
                result?.Longitude ?? 0.0
            );
        }
        catch (Exception)
        {
            // 回傳預設值而不是拋出例外
            return new LocationInfo("Unknown", "Unknown", "Unknown", 0.0, 0.0);
        }
    }

    /// <summary>
    /// 電子郵件驗證回應
    /// </summary>
    private record EmailValidationResponse(bool IsValid, string? Message);

    /// <summary>
    /// 地理位置回應
    /// </summary>
    private record LocationResponse(
        string Country,
        string City,
        string Region,
        double Latitude,
        double Longitude
    );
}