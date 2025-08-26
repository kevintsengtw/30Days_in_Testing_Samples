using System.Text.Json;

namespace Day17.Core;

/// <summary>
/// 負責配置檔案的讀取與寫入
/// </summary>
public class ConfigurationService
{
    private readonly IFileSystem _fileSystem;

    public ConfigurationService(IFileSystem fileSystem)
    {
        this._fileSystem = fileSystem;
    }

    /// <summary>
    /// 載入配置值，如果檔案不存在則回傳預設值
    /// </summary>
    /// <param name="filePath">配置檔案路徑</param>
    /// <param name="defaultValue">預設值</param>
    /// <returns>配置值</returns>
    public async Task<string> LoadConfigurationAsync(string filePath, string defaultValue = "")
    {
        if (!this._fileSystem.File.Exists(filePath))
        {
            return defaultValue;
        }

        try
        {
            return await this._fileSystem.File.ReadAllTextAsync(filePath);
        }
        catch (Exception)
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// 保存配置到檔案
    /// </summary>
    /// <param name="filePath">配置檔案路徑</param>
    /// <param name="value">要保存的值</param>
    public async Task SaveConfigurationAsync(string filePath, string value)
    {
        var directory = this._fileSystem.Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !this._fileSystem.Directory.Exists(directory))
        {
            this._fileSystem.Directory.CreateDirectory(directory);
        }

        await this._fileSystem.File.WriteAllTextAsync(filePath, value);
    }

    /// <summary>
    /// 載入 JSON 配置
    /// </summary>
    /// <typeparam name="T">配置類型</typeparam>
    /// <param name="filePath">配置檔案路徑</param>
    /// <returns>配置物件，如果檔案不存在或解析失敗則回傳預設值</returns>
    public async Task<T?> LoadJsonConfigurationAsync<T>(string filePath) where T : class
    {
        if (!this._fileSystem.File.Exists(filePath))
        {
            return default;
        }

        try
        {
            var jsonContent = await this._fileSystem.File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<T>(jsonContent);
        }
        catch (Exception)
        {
            return default;
        }
    }

    /// <summary>
    /// 保存 JSON 配置
    /// </summary>
    /// <typeparam name="T">配置類型</typeparam>
    /// <param name="filePath">配置檔案路徑</param>
    /// <param name="configuration">配置物件</param>
    public async Task SaveJsonConfigurationAsync<T>(string filePath, T configuration) where T : class
    {
        var directory = this._fileSystem.Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory) && !this._fileSystem.Directory.Exists(directory))
        {
            this._fileSystem.Directory.CreateDirectory(directory);
        }

        var jsonContent = JsonSerializer.Serialize(configuration, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await this._fileSystem.File.WriteAllTextAsync(filePath, jsonContent);
    }
}