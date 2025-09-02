namespace BookStore.Tests.Helpers;

/// <summary>
/// 用於接收 SQL 查詢結果的簡單 DTO
/// </summary>
public class QueryResult
{
    public int Value { get; set; }
}

/// <summary>
/// 用於接收日期時間查詢結果的 DTO
/// </summary>
public class DateTimeQueryResult
{
    public DateTime Value { get; set; }
}