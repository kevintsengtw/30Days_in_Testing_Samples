using MongoDB.Bson.Serialization.Attributes;

namespace Day22.Core.Models.Mongo;

/// <summary>
/// 地理位置 - GeoJSON 格式
/// </summary>
public class GeoLocation
{
    /// <summary>
    /// GeoJSON 類型，固定為 "Point"
    /// </summary>
    [BsonElement("type")]
    public string Type { get; set; } = "Point";

    /// <summary>
    /// 座標陣列，格式為 [經度, 緯度]
    /// </summary>
    [BsonElement("coordinates")]
    public double[] Coordinates { get; set; } = new double[2]; // [longitude, latitude]

    /// <summary>
    /// 建立 GeoJSON Point
    /// </summary>
    public static GeoLocation CreatePoint(double longitude, double latitude)
    {
        return new GeoLocation
        {
            Type = "Point",
            Coordinates = new[] { longitude, latitude }
        };
    }

    /// <summary>
    /// 經度
    /// </summary>
    [BsonIgnore]
    public double Longitude => Coordinates.Length > 0 ? Coordinates[0] : 0;

    /// <summary>
    /// 緯度
    /// </summary>
    [BsonIgnore]
    public double Latitude => Coordinates.Length > 1 ? Coordinates[1] : 0;
}