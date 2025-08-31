using MongoDB.Bson.Serialization.Attributes;

namespace Day22.Core.Models.Mongo;

/// <summary>
/// 技能模型 - 陣列查詢範例
/// </summary>
public class Skill
{
    /// <summary>
    /// 技能名稱
    /// </summary>
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 技能等級
    /// </summary>
    [BsonElement("level")]
    public SkillLevel Level { get; set; } = SkillLevel.Beginner;

    /// <summary>
    /// 經驗年數
    /// </summary>
    [BsonElement("years_experience")]
    public int YearsExperience { get; set; }

    /// <summary>
    /// 認證列表
    /// </summary>
    [BsonElement("certifications")]
    public List<string> Certifications { get; set; } = new();

    /// <summary>
    /// 是否經過驗證
    /// </summary>
    [BsonElement("verified")]
    public bool Verified { get; set; }
}