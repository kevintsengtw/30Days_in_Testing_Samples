using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Day22.Core.Models.Mongo;

/// <summary>
/// 技能等級列舉
/// </summary>
public enum SkillLevel
{
    /// <summary>
    /// 初學者
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    Beginner,

    /// <summary>
    /// 中級
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    Intermediate,

    /// <summary>
    /// 高級
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    Advanced,

    /// <summary>
    /// 專家
    /// </summary>
    [BsonRepresentation(BsonType.String)]
    Expert
}