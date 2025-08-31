using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Day22.Core.Models.Mongo;

/// <summary>
/// 使用者檔案 - 巢狀文件範例
/// </summary>
public class UserProfile
{
    [BsonElement("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [BsonElement("last_name")]
    public string LastName { get; set; } = string.Empty;

    [BsonElement("birth_date")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? BirthDate { get; set; }

    [BsonElement("bio")]
    public string Bio { get; set; } = string.Empty;

    [BsonElement("avatar_url")]
    public string AvatarUrl { get; set; } = string.Empty;

    [BsonElement("social_links")]
    public Dictionary<string, string> SocialLinks { get; set; } = new();

    /// <summary>
    /// 計算屬性 - 不會儲存到資料庫
    /// </summary>
    [BsonIgnore]
    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// 計算年齡 - 展示動態屬性
    /// </summary>
    [BsonIgnore]
    [JsonIgnore]
    public int? Age => BirthDate?.CalculateAge();
}