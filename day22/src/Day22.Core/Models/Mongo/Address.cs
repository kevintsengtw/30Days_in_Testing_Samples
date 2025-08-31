using MongoDB.Bson.Serialization.Attributes;

namespace Day22.Core.Models.Mongo;

/// <summary>
/// 地址模型 - 用於地理空間查詢
/// </summary>
public class Address
{
    /// <summary>
    /// 地址類型
    /// </summary>
    [BsonElement("type")]
    public string Type { get; set; } = string.Empty; // "home", "work", "other"

    /// <summary>
    /// 街道地址
    /// </summary>
    [BsonElement("street")]
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// 城市
    /// </summary>
    [BsonElement("city")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// 州/省
    /// </summary>
    [BsonElement("state")]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// 郵遞區號
    /// </summary>
    [BsonElement("postal_code")]
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// 國家
    /// </summary>
    [BsonElement("country")]
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// 地理位置 - 用於地理空間查詢
    /// </summary>
    [BsonElement("location")]
    public GeoLocation? Location { get; set; }

    /// <summary>
    /// 是否為主要地址
    /// </summary>
    [BsonElement("is_primary")]
    public bool IsPrimary { get; set; }
}