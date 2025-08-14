namespace Day05.Domain.Exceptions;

/// <summary>
/// class DatabaseConnectionException - 資料庫連線例外
/// </summary>
public class DatabaseConnectionException : Exception
{
    /// <summary>
    /// 初始化 <see cref="DatabaseConnectionException"/> 類別的新執行個體，並使用預設的錯誤訊息。
    /// </summary>
    public DatabaseConnectionException(string message = "資料庫連線發生錯誤") : base(message)
    {
    }

    /// <summary>
    /// 初始化 <see cref="DatabaseConnectionException"/> 類別的新執行個體，並使用指定的錯誤訊息和造成此例外狀況的內部例外狀況參考。
    /// </summary>
    /// <param name="message">描述錯誤的訊息。</param>
    /// <param name="innerException">造成目前例外狀況的例外狀況，如果未指定任何內部例外狀況，則為 null 參考。</param>
    public DatabaseConnectionException(string message, Exception innerException) : base(message, innerException)
    {
    }
}