using System.Collections.Concurrent;

// ReSharper disable InconsistentNaming

namespace Day13.Core.Misc;

/// <summary>
/// Interface Result
/// </summary>
public interface IResult
{
    /// <summary>
    /// 識別編號.
    /// </summary>
    /// <value>The identifier.</value>
    Guid ID { get; set; }

    /// <summary>
    /// 執行結果(true 成功, false 失敗).
    /// </summary>
    /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
    bool Success { get; set; }

    /// <summary>
    /// 執行結果訊息.
    /// </summary>
    /// <value>The message.</value>
    string Message { get; set; }

    /// <summary>
    /// 執行結果影響資料筆數
    /// </summary>
    /// <value>The affect rows.</value>
    int AffectRows { get; set; }

    /// <summary>
    /// 例外.
    /// </summary>
    /// <value>The exception.</value>
    Exception Exception { get; set; }

    /// <summary>
    /// Inner Results.
    /// </summary>
    /// <value>The inner results.</value>
    ConcurrentBag<IResult> InnerResults { get; }

    /// <summary>
    /// Adds Inner Results.
    /// </summary>
    /// <param name="innerResult">The inner result.</param>
    /// <returns>IResult.</returns>
    IResult AddResult(IResult innerResult);

    /// <summary>
    /// Serializes this instance.
    /// </summary>
    /// <returns>System.String.</returns>
    string Serialize();
}

/// <summary>
/// Interface IResult for GenericType
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IResult<T>
{
    /// <summary>
    /// 識別編號.
    /// </summary>
    /// <value>The identifier.</value>
    Guid ID { get; set; }

    /// <summary>
    /// 執行結果(true 成功, false 失敗).
    /// </summary>
    /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
    bool Success { get; set; }

    /// <summary>
    /// 執行結果訊息.
    /// </summary>
    /// <value>The message.</value>
    string Message { get; set; }

    /// <summary>
    /// 執行結果影響資料筆數
    /// </summary>
    /// <value>The affect rows.</value>
    int AffectRows { get; set; }

    /// <summary>
    /// 例外.
    /// </summary>
    /// <value>The exception.</value>
    Exception Exception { get; set; }

    /// <summary>
    /// Inner Results.
    /// </summary>
    /// <value>The inner results.</value>
    ConcurrentBag<IResult<T>> InnerResults { get; }

    /// <summary>
    /// 回傳值
    /// </summary>
    T ReturnValue { get; set; }

    /// <summary>
    /// Adds Inner Results.
    /// </summary>
    /// <param name="innerResult">The inner result.</param>
    /// <returns>IResult.</returns>
    IResult<T> AddResult(IResult<T> innerResult);

    /// <summary>
    /// Serializes this instance.
    /// </summary>
    /// <returns>System.String.</returns>
    string Serialize();
}