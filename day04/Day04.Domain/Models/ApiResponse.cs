using System.Net;

namespace Day04.Domain.Models;

/// <summary>
/// class ApiResponse - 用於API回應的模型。
/// </summary>
/// <typeparam name="T"></typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// API回應的狀態碼
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// API回應的資料
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// API回應的處理時間
    /// </summary>
    /// <value></value>
    public TimeSpan ResponseTime { get; set; }

    /// <summary>
    /// API回應的錯誤訊息
    /// </summary>
    /// <value></value>
    public string? ErrorMessage { get; set; }
}