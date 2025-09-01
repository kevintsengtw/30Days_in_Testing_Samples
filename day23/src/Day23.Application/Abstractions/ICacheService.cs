namespace Day23.Application.Abstractions;

/// <summary>
/// 快取服務介面
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// 取得字串值
    /// </summary>
    Task<string?> GetStringAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 設定字串值
    /// </summary>
    Task SetStringAsync(string key, string value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除快取
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根據前綴移除快取
    /// </summary>
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
}