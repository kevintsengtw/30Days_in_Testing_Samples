namespace Day20.Core.Services;

/// <summary>
/// 外部 API 服務介面
/// </summary>
public interface IExternalApiService
{
    /// <summary>
    /// 驗證使用者電子郵件地址
    /// </summary>
    /// <param name="email">電子郵件地址</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns>驗證結果</returns>
    Task<bool> ValidateEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取得使用者的地理位置資訊
    /// </summary>
    /// <param name="ipAddress">IP 地址</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns>地理位置資訊</returns>
    Task<LocationInfo> GetLocationAsync(string ipAddress, CancellationToken cancellationToken = default);
}

/// <summary>
/// 地理位置資訊
/// </summary>
public record LocationInfo(
    string Country,
    string City,
    string Region,
    double Latitude,
    double Longitude
);