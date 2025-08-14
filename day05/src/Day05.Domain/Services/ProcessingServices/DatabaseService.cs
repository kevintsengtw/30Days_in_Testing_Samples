using Day05.Domain.Exceptions;

namespace Day05.Domain.Services.ProcessingServices;

/// <summary>
/// class DatabaseService - 數據庫服務
/// </summary>
public class DatabaseService
{
    /// <summary>
    /// 連接到數據庫
    /// </summary>
    /// <param name="connectionString">連接字串</param>
    public void Connect(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString) || connectionString.Contains("invalid"))
        {
            throw new DatabaseConnectionException("無效的連線字串", new ArgumentException("連線字串不可為無效值"));
        }
    }
}