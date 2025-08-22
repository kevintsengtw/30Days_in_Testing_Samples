using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

// ReSharper disable StaticMemberInGenericType

namespace Day13.Core.Misc;

/// <inheritdoc />
/// <summary>
/// Class Result.
/// </summary>
[Serializable]
public class Result : IResult
{
    /// <summary>
    /// 識別編號.
    /// </summary>
    /// <value>The identifier.</value>
    public Guid ID { get; set; }

    private bool _success;

    /// <summary>
    /// 執行結果(true 成功, false 失敗).
    /// </summary>
    /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
    public bool Success
    {
        get => this.InnerResults.Cast<Result>().Where(x => x != null).All(x => x.Success) && this._success;
        set => this._success = value;
    }

    private string _message;

    /// <summary>
    /// 執行結果訊息.
    /// </summary>
    /// <value>The message.</value>
    public string Message
    {
        get
        {
            var sb = new StringBuilder();
            sb.Append(this._message);

            foreach (var result1 in this.InnerResults)
            {
                var result = (Result)result1;
                if (result.Success)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(result.Message))
                {
                    continue;
                }

                sb.AppendLine();
                sb.Append(result.Message);
            }

            return sb.ToString();
        }
        set => this._message = value;
    }

    /// <summary>
    /// 執行結果影響資料筆數
    /// </summary>
    /// <value>The affect rows.</value>
    public int AffectRows { get; set; }

    /// <summary>
    /// 例外.
    /// </summary>
    /// <value>The exception.</value>
    public Exception Exception { get; set; }

    /// <summary>
    /// Inner Results.
    /// </summary>
    /// <value>The inner results.</value>
    public ConcurrentBag<IResult> InnerResults { get; protected set; }

    public Result()
        : this(false)
    {
    }

    public Result(bool success)
    {
        this.ID = Guid.NewGuid();
        this.Success = success;
        this.InnerResults = [];
    }

    //-----------------------------------------------------------------------------------------

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString()
    {
        return this.Success ? "Success" : "Failure";
    }

    /// <summary>
    /// Adds Inner Results.
    /// </summary>
    /// <param name="innerResult">The inner result.</param>
    /// <returns>IResult.</returns>
    public IResult AddResult(IResult innerResult)
    {
        this.InnerResults.Add(innerResult);
        return this;
    }

    /// <summary>
    /// Serializes this instance.
    /// </summary>
    /// <returns>System.String.</returns>
    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }
}

/// <summary>
/// Class Result for 泛型且需要帶回傳值.
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="IResult" />
[Serializable]
public class Result<T> : IResult<T>
{
    /// <summary>
    /// 識別編號.
    /// </summary>
    /// <value>The identifier.</value>
    public Guid ID { get; set; }

    private bool _success;

    /// <summary>
    /// 執行結果(true 成功, false 失敗).
    /// </summary>
    /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
    public bool Success
    {
        get => this.InnerResults.Cast<Result<T>>().Where(x => x != null).All(x => x.Success) && this._success;
        set => this._success = value;
    }

    /// <summary>
    /// 執行結果訊息.
    /// </summary>
    /// <value>The message.</value>
    public string Message { get; set; }

    /// <summary>
    /// 執行結果影響資料筆數
    /// </summary>
    /// <value>The affect rows.</value>
    public int AffectRows { get; set; }

    /// <summary>
    /// 例外.
    /// </summary>
    /// <value>The exception.</value>
    public Exception Exception { get; set; }

    /// <summary>
    /// Inner Results.
    /// </summary>
    /// <value>The inner results.</value>
    public ConcurrentBag<IResult<T>> InnerResults { get; protected set; }

    /// <summary>
    /// 回傳值
    /// </summary>
    /// <value>The return value.</value>
    public T ReturnValue { get; set; }

    public Result()
        : this(false)
    {
    }

    public Result(bool success)
    {
        this.ID = Guid.NewGuid();
        this.Success = success;
        this.InnerResults = new ConcurrentBag<IResult<T>>();
    }

    //-----------------------------------------------------------------------------------------

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString()
    {
        return this.Success ? "Success" : "Failure";
    }

    /// <summary>
    /// Adds the result.
    /// </summary>
    /// <param name="innerResult">The inner result.</param>
    /// <returns>IResult&lt;T&gt;.</returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public IResult<T> AddResult(IResult<T> innerResult)
    {
        this.InnerResults.Add(innerResult);
        return this;
    }

    /// <summary>
    /// Serializes this instance.
    /// </summary>
    /// <returns>System.String.</returns>
    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }
}