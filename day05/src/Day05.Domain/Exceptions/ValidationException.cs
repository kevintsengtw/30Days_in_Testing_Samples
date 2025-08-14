namespace Day05.Domain.Exceptions;

/// <summary>
/// class ValidationException - 資料驗證例外
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// 初始化 <see cref="ValidationException"/> 類別的新執行個體，並使用預設的錯誤訊息。
    /// </summary>
    public ValidationException(string message = "資料驗證失敗") : base(message)
    {
    }
}