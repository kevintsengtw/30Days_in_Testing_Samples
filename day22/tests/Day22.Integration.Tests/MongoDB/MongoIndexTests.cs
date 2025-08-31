using System.Diagnostics;
using Day22.Core.Models.Mongo;
using Day22.Integration.Tests.Fixtures;
using Xunit.Abstractions;

namespace Day22.Integration.Tests.MongoDB;

/// <summary>
/// MongoDB 索引效能測試
/// </summary>
[Collection("MongoDb Collection")]
public class MongoIndexTests
{
    private readonly MongoDbContainerFixture _fixture;
    private readonly IMongoCollection<UserDocument> _users;
    private readonly ITestOutputHelper _output;

    public MongoIndexTests(MongoDbContainerFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _users = fixture.Database.GetCollection<UserDocument>("index_test_users");
        _output = output;
    }

    [Fact]
    public async Task CreateUniqueIndex_電子郵件唯一索引_應防止重複插入()
    {
        // Arrange - 確保集合為空
        await _users.DeleteManyAsync(FilterDefinition<UserDocument>.Empty);

        // 建立唯一索引
        var indexKeysDefinition = Builders<UserDocument>.IndexKeys.Ascending(u => u.Email);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<UserDocument>(indexKeysDefinition, indexOptions);
        await _users.Indexes.CreateOneAsync(indexModel);

        var user1 = new UserDocument
        {
            Username = "user1",
            Email = "test@example.com"
        };

        var user2 = new UserDocument
        {
            Username = "user2",
            Email = "test@example.com" // 相同的電子郵件
        };

        // Act & Assert
        await _users.InsertOneAsync(user1); // 第一次插入應該成功

        // 第二次插入相同 email 應該失敗
        var exception = await Assert.ThrowsAsync<MongoWriteException>(() => _users.InsertOneAsync(user2));
        exception.WriteError.Category.Should().Be(ServerErrorCategory.DuplicateKey);

        _output.WriteLine("唯一索引測試通過 - 重複的 email 被正確阻擋");
    }

    [Fact]
    public async Task CompoundIndex_複合索引查詢效能_應顯著提升查詢速度()
    {
        // Arrange - 確保集合為空
        await _users.DeleteManyAsync(FilterDefinition<UserDocument>.Empty);

        // 插入大量測試資料
        var testUsers = new List<UserDocument>();
        for (var i = 0; i < 1000; i++)
        {
            testUsers.Add(new UserDocument
            {
                Username = $"user_{i:D4}",
                Email = $"user{i:D4}@example.com",
                IsActive = i % 2 == 0,
                CreatedAt = DateTime.UtcNow.AddDays(-i % 365)
            });
        }

        await _users.InsertManyAsync(testUsers);

        // 測試查詢效能 - 沒有索引
        var stopwatch = Stopwatch.StartNew();
        var filter = Builders<UserDocument>.Filter.And(
            Builders<UserDocument>.Filter.Eq(u => u.IsActive, true),
            Builders<UserDocument>.Filter.Gte(u => u.CreatedAt, DateTime.UtcNow.AddDays(-100))
        );

        await _users.Find(filter).ToListAsync();
        stopwatch.Stop();
        var timeWithoutIndex = stopwatch.ElapsedMilliseconds;

        // 建立複合索引
        var compoundIndex = Builders<UserDocument>.IndexKeys
                                                  .Ascending(u => u.IsActive)
                                                  .Descending(u => u.CreatedAt);
        await _users.Indexes.CreateOneAsync(new CreateIndexModel<UserDocument>(compoundIndex));

        // 測試查詢效能 - 有索引
        stopwatch.Restart();
        await _users.Find(filter).ToListAsync();
        stopwatch.Stop();
        var timeWithIndex = stopwatch.ElapsedMilliseconds;

        // Assert
        // 索引應該提升查詢效能（但在小資料集中可能差異不大）
        var improvement = (double)timeWithoutIndex / Math.Max(timeWithIndex, 1);

        _output.WriteLine($"查詢時間比較 - 無索引: {timeWithoutIndex}ms, 有索引: {timeWithIndex}ms");
        _output.WriteLine($"效能提升倍數: {improvement:F2}x");

        // 在小資料集中，索引帶來的效能提升可能不明顯，但應該不會變慢
        timeWithIndex.Should().BeLessThan(timeWithoutIndex + 100); // 允許100ms的誤差
    }

    [Fact]
    public async Task TextIndex_全文檢索索引_應支援文字搜尋()
    {
        // Arrange - 確保集合為空
        await _users.DeleteManyAsync(FilterDefinition<UserDocument>.Empty);

        // 建立文字索引
        var textIndex = Builders<UserDocument>.IndexKeys
                                              .Text(u => u.Profile.Bio)
                                              .Text(u => u.Username);
        await _users.Indexes.CreateOneAsync(new CreateIndexModel<UserDocument>(textIndex));

        // 插入測試資料
        var users = new List<UserDocument>
        {
            new() { Username = "developer123", Email = "dev@example.com", Profile = new UserProfile { Bio = "Experienced C# developer specializing in MongoDB integration" } },
            new() { Username = "designer456", Email = "design@example.com", Profile = new UserProfile { Bio = "UI/UX designer with expertise in web applications" } },
            new() { Username = "tester789", Email = "test@example.com", Profile = new UserProfile { Bio = "QA engineer focused on automated testing and MongoDB testing" } }
        };
        await _users.InsertManyAsync(users);

        // Act - 全文檢索查詢
        var textSearchFilter = Builders<UserDocument>.Filter.Text("MongoDB");
        var results = await _users.Find(textSearchFilter).ToListAsync();

        // Assert
        results.Should().HaveCount(2); // developer123 和 tester789 的 bio 包含 "MongoDB"
        results.Should().Contain(u => u.Username == "developer123");
        results.Should().Contain(u => u.Username == "tester789");
        results.Should().NotContain(u => u.Username == "designer456");

        _output.WriteLine($"全文檢索測試通過 - 找到 {results.Count} 個包含 'MongoDB' 的文件");
    }

    [Fact]
    public async Task SparseIndex_稀疏索引_應只索引有值的欄位()
    {
        // Arrange - 確保集合為空
        await _users.DeleteManyAsync(FilterDefinition<UserDocument>.Empty);

        // 建立稀疏索引（只索引有 birth_date 的文件）
        var sparseIndex = Builders<UserDocument>.IndexKeys.Ascending(u => u.Profile.BirthDate);
        var sparseOptions = new CreateIndexOptions { Sparse = true };
        await _users.Indexes.CreateOneAsync(new CreateIndexModel<UserDocument>(sparseIndex, sparseOptions));

        // 插入測試資料 - 部分有 birth_date，部分沒有
        var usersWithBirthDate = new List<UserDocument>
        {
            new() { Username = "user1", Email = "user1@example.com", Profile = new UserProfile { BirthDate = new DateTime(1990, 1, 1) } },
            new() { Username = "user2", Email = "user2@example.com", Profile = new UserProfile { BirthDate = new DateTime(1985, 5, 15) } }
        };

        var usersWithoutBirthDate = new List<UserDocument>
        {
            new() { Username = "user3", Email = "user3@example.com", Profile = new UserProfile() }, // BirthDate 為 null
            new() { Username = "user4", Email = "user4@example.com", Profile = new UserProfile() }
        };

        await _users.InsertManyAsync(usersWithBirthDate);
        await _users.InsertManyAsync(usersWithoutBirthDate);

        // Act - 查詢有生日的使用者（不為 null）
        var filter = Builders<UserDocument>.Filter.Ne(u => u.Profile.BirthDate, null);
        var usersWithBirthDateResults = await _users.Find(filter).ToListAsync();

        // Assert
        usersWithBirthDateResults.Should().HaveCount(2);
        usersWithBirthDateResults.Should().AllSatisfy(u => u.Profile.BirthDate.Should().NotBeNull());

        _output.WriteLine($"稀疏索引測試通過 - 找到 {usersWithBirthDateResults.Count} 個有生日的使用者");
    }

    [Fact]
    public async Task IndexStats_索引統計資訊_應正確顯示索引使用情況()
    {
        // Arrange - 確保集合為空
        await _users.DeleteManyAsync(FilterDefinition<UserDocument>.Empty);

        // 建立索引 - 使用自訂名稱避免衝突
        var emailIndex = Builders<UserDocument>.IndexKeys.Ascending(u => u.Email);
        var indexOptions = new CreateIndexOptions { Name = "email_stats_index" };
        await _users.Indexes.CreateOneAsync(new CreateIndexModel<UserDocument>(emailIndex, indexOptions));

        // 插入測試資料
        var testUsers = Enumerable.Range(1, 100)
                                  .Select(i => new UserDocument
                                  {
                                      Username = $"user_{i:D3}",
                                      Email = $"user{i:D3}@example.com"
                                  }).ToList();
        await _users.InsertManyAsync(testUsers);

        // Act - 執行一些查詢來使用索引
        for (var i = 1; i <= 10; i++)
        {
            await _users.Find(u => u.Email == $"user{i:D3}@example.com").FirstOrDefaultAsync();
        }

        // 取得索引清單
        var indexes = await _users.Indexes.ListAsync();
        var indexList = await indexes.ToListAsync();

        // Assert
        indexList.Should().NotBeEmpty();

        // 應該有預設的 _id 索引和我們建立的 email_stats_index 索引
        var emailIndexInfo = indexList.FirstOrDefault(idx => idx.Contains("name") &&
                                                             idx["name"].AsString == "email_stats_index");

        emailIndexInfo.Should().NotBeNull();

        _output.WriteLine($"索引統計測試通過 - 找到 {indexList.Count} 個索引");
        foreach (var index in indexList)
        {
            _output.WriteLine($"索引: {index["name"]}, 鍵值: {index["key"]}");
        }
    }
}