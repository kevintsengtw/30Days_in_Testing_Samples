using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Day22.Core.Models.Mongo;

/// <summary>
/// MongoDB 使用者文件 - 展示完整的文件結構設計
/// </summary>
public class UserDocument
{
    /// <summary>
    /// Id
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 使用者名稱
    /// </summary>
    [BsonElement("username")]
    [BsonRequired]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 電子郵件
    /// </summary>
    [BsonElement("email")]
    [BsonRequired]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// 個人資料
    /// </summary>
    [BsonElement("profile")]
    public UserProfile Profile { get; set; } = new();

    /// <summary>
    /// 地址列表
    /// </summary>
    [BsonElement("addresses")]
    public List<Address> Addresses { get; set; } = new();

    /// <summary>
    /// 技能列表
    /// </summary>
    [BsonElement("skills")]
    public List<Skill> Skills { get; set; } = new();

    /// <summary>
    /// 偏好設定
    /// </summary>
    [BsonElement("preferences")]
    public Dictionary<string, object> Preferences { get; set; } = new();

    /// <summary>
    /// 建立時間
    /// </summary>
    [BsonElement("created_at")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    [BsonElement("updated_at")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    [BsonElement("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 文件版本號，用於樂觀鎖定
    /// </summary>
    [BsonElement("version")]
    public int Version { get; set; } = 1;

    /// <summary>
    /// 用於展示文件更新的樂觀鎖定
    /// </summary>
    /// <param name="updateTime">更新時間</param>
    public void IncrementVersion(DateTime updateTime)
    {
        Version++;
        UpdatedAt = updateTime;
    }
}