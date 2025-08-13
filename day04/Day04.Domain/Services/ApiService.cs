namespace Day04.Domain.Services;

/// <summary>
/// class ApiService - 用於調用外部 API 的服務。
/// </summary>
public class ApiService
{
    /// <summary>
    /// 獲取資料
    /// </summary>
    /// <param name="endpoint">API 端點</param>
    /// <returns>API 回傳的資料</returns>
    public async Task<string> GetDataAsync(string endpoint)
    {
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new ArgumentException("Endpoint 不能為空", nameof(endpoint));
        }

        if (endpoint.Contains("invalid"))
        {
            throw new HttpRequestException("404 - 找不到 Endpoint");
        }

        // 模擬 API 呼叫
        await Task.Delay(200);
        return $"Data from {endpoint}";
    }
}