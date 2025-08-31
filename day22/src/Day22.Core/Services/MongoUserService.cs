using Day22.Core.Configuration;
using Day22.Core.Models.Mongo;
using MongoDB.Bson;

namespace Day22.Core.Services;

/// <summary>
/// MongoDB 使用者服務 - 展示完整的 MongoDB 操作
/// </summary>
public class MongoUserService : IUserService
{
    private readonly IMongoCollection<UserDocument> _users;
    private readonly ILogger<MongoUserService> _logger;
    private readonly MongoDbSettings _settings;
    private readonly TimeProvider _timeProvider;

    public MongoUserService(
        IMongoDatabase database,
        IOptions<MongoDbSettings> settings,
        ILogger<MongoUserService> logger,
        TimeProvider timeProvider)
    {
        _settings = settings.Value;
        _users = database.GetCollection<UserDocument>(_settings.UsersCollectionName);
        _logger = logger;
        _timeProvider = timeProvider;

        // 建立索引
        CreateIndexesAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// 建立必要的索引
    /// </summary>
    private async Task CreateIndexesAsync()
    {
        try
        {
            var indexKeysDefinition = Builders<UserDocument>.IndexKeys
                                                            .Ascending(x => x.Email)
                                                            .Ascending(x => x.Username);

            var indexOptions = new CreateIndexOptions
            {
                Unique = true,
                Name = "email_username_unique"
            };

            await _users.Indexes.CreateOneAsync(new CreateIndexModel<UserDocument>(indexKeysDefinition, indexOptions));

            // 地理空間索引
            var geoIndexKeys = Builders<UserDocument>.IndexKeys.Geo2DSphere("addresses.location");

            await _users.Indexes.CreateOneAsync(
                new CreateIndexModel<UserDocument>(
                    geoIndexKeys,
                    new CreateIndexOptions { Name = "addresses_location_2dsphere" }));

            // 技能索引
            var skillIndexKeys = Builders<UserDocument>.IndexKeys
                                                       .Ascending("skills.name")
                                                       .Ascending("skills.level");

            await _users.Indexes.CreateOneAsync(
                new CreateIndexModel<UserDocument>(
                    skillIndexKeys, 
                    new CreateIndexOptions { Name = "skills_compound" }));

            _logger.LogInformation("MongoDB 索引建立完成");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "建立 MongoDB 索引時發生錯誤");
        }
    }

    /// <summary>
    /// 新增使用者
    /// </summary>
    public async Task<UserDocument> CreateUserAsync(UserDocument user)
    {
        try
        {
            var now = _timeProvider.GetUtcNow().DateTime;
            user.CreatedAt = now;
            user.UpdatedAt = now;
            user.Version = 1;

            await _users.InsertOneAsync(user);
            _logger.LogInformation("成功建立使用者: {UserId}", user.Id);
            return user;
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            _logger.LogWarning("使用者已存在: {Email}", user.Email);
            throw new InvalidOperationException($"使用者 {user.Email} 已存在");
        }
    }

    /// <summary>
    /// 根據 ID 取得使用者
    /// </summary>
    public async Task<UserDocument?> GetUserByIdAsync(string id)
    {
        var filter = Builders<UserDocument>.Filter.Eq(x => x.Id, id);
        return await _users.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// 根據電子郵件取得使用者
    /// </summary>
    public async Task<UserDocument?> GetUserByEmailAsync(string email)
    {
        var filter = Builders<UserDocument>.Filter.Eq(x => x.Email, email);
        return await _users.Find(filter).FirstOrDefaultAsync();
    }

    /// <summary>
    /// 取得所有使用者（支援分頁）
    /// </summary>
    public async Task<List<UserDocument>> GetAllUsersAsync(int skip = 0, int limit = 100)
    {
        return await _users.Find(FilterDefinition<UserDocument>.Empty)
                           .Skip(skip)
                           .Limit(limit)
                           .SortBy(x => x.Username)
                           .ToListAsync();
    }

    /// <summary>
    /// 更新使用者（使用樂觀鎖定）
    /// </summary>
    public async Task<UserDocument?> UpdateUserAsync(UserDocument user)
    {
        var filter = Builders<UserDocument>.Filter.And(
            Builders<UserDocument>.Filter.Eq(x => x.Id, user.Id),
            Builders<UserDocument>.Filter.Eq(x => x.Version, user.Version)
        );

        user.IncrementVersion(_timeProvider.GetUtcNow().DateTime);

        var result = await _users.ReplaceOneAsync(filter, user);

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException("使用者不存在或版本衝突");
        }

        _logger.LogInformation("成功更新使用者: {UserId}, 版本: {Version}", user.Id, user.Version);
        return user;
    }

    /// <summary>
    /// 部分更新使用者檔案
    /// </summary>
    public async Task<bool> UpdateUserProfileAsync(string userId, UserProfile profile)
    {
        var filter = Builders<UserDocument>.Filter.Eq(x => x.Id, userId);
        var update = Builders<UserDocument>.Update
                                           .Set(x => x.Profile, profile)
                                           .Set(x => x.UpdatedAt, _timeProvider.GetUtcNow().DateTime)
                                           .Inc(x => x.Version, 1);

        var result = await _users.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    /// <summary>
    /// 新增使用者地址
    /// </summary>
    public async Task<bool> AddUserAddressAsync(string userId, Address address)
    {
        var filter = Builders<UserDocument>.Filter.Eq(x => x.Id, userId);
        var update = Builders<UserDocument>.Update
                                           .Push(x => x.Addresses, address)
                                           .Set(x => x.UpdatedAt, _timeProvider.GetUtcNow().DateTime)
                                           .Inc(x => x.Version, 1);

        var result = await _users.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    /// <summary>
    /// 新增使用者技能
    /// </summary>
    public async Task<bool> AddUserSkillAsync(string userId, Skill skill)
    {
        var filter = Builders<UserDocument>.Filter.And(
            Builders<UserDocument>.Filter.Eq(x => x.Id, userId),
            Builders<UserDocument>.Filter.Not(
                Builders<UserDocument>.Filter.ElemMatch(x => x.Skills, s => s.Name == skill.Name)
            )
        );

        var update = Builders<UserDocument>.Update
                                           .Push(x => x.Skills, skill)
                                           .Set(x => x.UpdatedAt, _timeProvider.GetUtcNow().DateTime)
                                           .Inc(x => x.Version, 1);

        var result = await _users.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    /// <summary>
    /// 更新使用者技能等級
    /// </summary>
    public async Task<bool> UpdateUserSkillLevelAsync(string userId, string skillName, SkillLevel level)
    {
        var filter = Builders<UserDocument>.Filter.And(
            Builders<UserDocument>.Filter.Eq(x => x.Id, userId),
            Builders<UserDocument>.Filter.ElemMatch(x => x.Skills, s => s.Name == skillName)
        );

        var update = Builders<UserDocument>.Update
                                           .Set("skills.$.level", level)
                                           .Set(x => x.UpdatedAt, _timeProvider.GetUtcNow().DateTime)
                                           .Inc(x => x.Version, 1);

        var result = await _users.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    /// <summary>
    /// 地理空間查詢 - 找出附近的使用者
    /// </summary>
    public async Task<List<UserDocument>> FindUsersNearLocationAsync(
        double longitude, double latitude, double maxDistanceKm = 10)
    {
        var location = GeoLocation.CreatePoint(longitude, latitude);

        var filter = Builders<UserDocument>.Filter.Near(
            "addresses.location.coordinates",
            longitude, latitude,
            maxDistanceKm * 1000); // 轉換為公尺

        return await _users.Find(filter).ToListAsync();
    }

    /// <summary>
    /// 聚合查詢 - 按技能等級統計使用者
    /// </summary>
    public async Task<List<BsonDocument>> GetUserSkillStatisticsAsync()
    {
        var pipeline = new[]
        {
            new BsonDocument("$unwind", "$skills"),
            new BsonDocument("$group", new BsonDocument
            {
                ["_id"] = new BsonDocument
                {
                    ["skill"] = "$skills.name",
                    ["level"] = "$skills.level"
                },
                ["count"] = new BsonDocument("$sum", 1),
                ["avgExperience"] = new BsonDocument("$avg", "$skills.years_experience")
            }),
            new BsonDocument("$sort", new BsonDocument("count", -1))
        };

        return await _users.Aggregate<BsonDocument>(pipeline).ToListAsync();
    }

    /// <summary>
    /// 文字搜尋
    /// </summary>
    public async Task<List<UserDocument>> SearchUsersAsync(string searchText)
    {
        var filter = Builders<UserDocument>.Filter.Or(
            Builders<UserDocument>.Filter.Regex(x => x.Username, new BsonRegularExpression(searchText, "i")),
            Builders<UserDocument>.Filter.Regex(x => x.Email, new BsonRegularExpression(searchText, "i")),
            Builders<UserDocument>.Filter.Regex(x => x.Profile.Bio, new BsonRegularExpression(searchText, "i"))
        );

        return await _users.Find(filter).ToListAsync();
    }

    /// <summary>
    /// 刪除使用者
    /// </summary>
    public async Task<bool> DeleteUserAsync(string id)
    {
        var filter = Builders<UserDocument>.Filter.Eq(x => x.Id, id);
        var result = await _users.DeleteOneAsync(filter);

        if (result.DeletedCount > 0)
        {
            _logger.LogInformation("成功刪除使用者: {UserId}", id);
        }

        return result.DeletedCount > 0;
    }

    /// <summary>
    /// 批次新增使用者
    /// </summary>
    public async Task<int> CreateUsersAsync(IEnumerable<UserDocument> users)
    {
        var userList = users.ToList();

        // 設定建立時間和版本
        var now = _timeProvider.GetUtcNow().DateTime;
        foreach (var user in userList)
        {
            user.CreatedAt = now;
            user.UpdatedAt = now;
            user.Version = 1;
        }

        try
        {
            await _users.InsertManyAsync(userList);
            _logger.LogInformation("成功批次建立 {Count} 個使用者", userList.Count);
            return userList.Count;
        }
        catch (MongoBulkWriteException ex)
        {
            var successCount = userList.Count - ex.WriteErrors.Count;
            _logger.LogWarning("批次建立使用者部分失敗，成功: {Success}, 失敗: {Failed}",
                               successCount, ex.WriteErrors.Count);
            return successCount;
        }
    }

    /// <summary>
    /// 取得使用者總數
    /// </summary>
    public async Task<long> GetUserCountAsync()
    {
        return await _users.CountDocumentsAsync(FilterDefinition<UserDocument>.Empty);
    }

    /// <summary>
    /// 檢查使用者是否存在
    /// </summary>
    public async Task<bool> UserExistsAsync(string email)
    {
        var filter = Builders<UserDocument>.Filter.Eq(x => x.Email, email);
        var count = await _users.CountDocumentsAsync(filter, new CountOptions { Limit = 1 });
        return count > 0;
    }

    /// <summary>
    /// 停用使用者
    /// </summary>
    public async Task<bool> DeactivateUserAsync(string id)
    {
        var filter = Builders<UserDocument>.Filter.Eq(x => x.Id, id);
        var update = Builders<UserDocument>.Update
                                           .Set(x => x.IsActive, false)
                                           .Set(x => x.UpdatedAt, DateTime.UtcNow)
                                           .Inc(x => x.Version, 1);

        var result = await _users.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    /// <summary>
    /// 啟用使用者
    /// </summary>
    public async Task<bool> ActivateUserAsync(string id)
    {
        var filter = Builders<UserDocument>.Filter.Eq(x => x.Id, id);
        var update = Builders<UserDocument>.Update
                                           .Set(x => x.IsActive, true)
                                           .Set(x => x.UpdatedAt, DateTime.UtcNow)
                                           .Inc(x => x.Version, 1);

        var result = await _users.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }
}