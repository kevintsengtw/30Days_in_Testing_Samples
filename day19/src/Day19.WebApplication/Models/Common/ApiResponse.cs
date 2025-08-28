namespace Day19.WebApplication.Models.Common;

/// <summary>
/// 統一的 API 回應格式
/// </summary>
/// <typeparam name="T">資料類型</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 方法名稱
    /// </summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// 回應資料
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 錯誤資訊
    /// </summary>
    public ErrorInfo? Error { get; set; }
}

/// <summary>
/// 錯誤資訊
/// </summary>
public class ErrorInfo
{
    /// <summary>
    /// 錯誤代碼
    /// </summary>
    public int ErrorCode { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 錯誤描述
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// 成功回應的建立者
/// </summary>
public static class ApiResponse
{
    /// <summary>
    /// 建立成功回應
    /// </summary>
    public static ApiResponse<T> Success<T>(T data, string method = "")
    {
        return new ApiResponse<T>
        {
            Status = "Success",
            Method = method,
            Data = data
        };
    }

    /// <summary>
    /// 建立失敗回應
    /// </summary>
    public static ApiResponse<object> Fail(int errorCode, string message, string description = "", string method = "")
    {
        return new ApiResponse<object>
        {
            Status = "Error",
            Method = method,
            Error = new ErrorInfo
            {
                ErrorCode = errorCode,
                Message = message,
                Description = description
            }
        };
    }

    /// <summary>
    /// 建立驗證錯誤回應
    /// </summary>
    public static ApiResponse<object> ValidationError(string message, string description = "", string method = "")
    {
        return new ApiResponse<object>
        {
            Status = "ValidationError",
            Method = method,
            Error = new ErrorInfo
            {
                ErrorCode = 30001,
                Message = message,
                Description = description
            }
        };
    }
}