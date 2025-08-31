using Day22.Core.Models.Mongo;
using Day22.Integration.Tests.Fixtures;
using MongoDB.Bson;
using Xunit.Abstractions;

namespace Day22.Integration.Tests.MongoDB;

/// <summary>
/// MongoDB BSON 處理測試
/// </summary>
[Collection("MongoDb Collection")]
public class MongoBsonTests
{
    private readonly MongoDbContainerFixture _fixture;
    private readonly IMongoCollection<UserDocument> _users;
    private readonly ITestOutputHelper _output;

    public MongoBsonTests(MongoDbContainerFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _users = fixture.Database.GetCollection<UserDocument>("bson_test_users");
        _output = output;
    }

    [Fact]
    public async Task BsonObjectIdGeneration_自動產生ObjectId_應為唯一值()
    {
        // Arrange & Act
        var users = new List<UserDocument>();
        for (var i = 0; i < 100; i++)
        {
            var user = new UserDocument
            {
                Username = $"user_{i:D3}",
                Email = $"user{i:D3}@example.com"
            };
            await _users.InsertOneAsync(user);
            users.Add(user);
        }

        // Assert
        var allIds = users.Select(u => u.Id).ToList();
        allIds.Should().HaveCount(100);
        allIds.Should().OnlyHaveUniqueItems();

        // 驗證 ObjectId 格式
        foreach (var id in allIds)
        {
            ObjectId.TryParse(id, out var objectId).Should().BeTrue();
        }

        _output.WriteLine($"成功產生 {allIds.Count} 個唯一的 ObjectId");
    }

    [Fact]
    public async Task BsonNullAndMissingFields_處理空值和遺失欄位_應正確處理()
    {
        // Arrange - 建立包含 null 值的文件
        var userWithNulls = new BsonDocument
        {
            { "username", "test_user" },
            { "email", "test@example.com" },
            {
                "profile", new BsonDocument
                {
                    { "first_name", "Test" },
                    { "last_name", BsonNull.Value }, // 明確設定為 null
                    // birth_date 欄位完全省略
                    { "bio", "" }
                }
            },
            { "skills", new BsonArray() },         // 空陣列
            { "preferences", new BsonDocument() }, // 空物件
            { "created_at", DateTime.UtcNow },
            { "updated_at", DateTime.UtcNow },
            { "is_active", true }
        };

        // Act - 插入 BSON 文件
        await _users.Database.GetCollection<BsonDocument>("bson_test_users").InsertOneAsync(userWithNulls);

        // 讀取為強型別物件
        var retrievedUser = await _users
                                  .Find(u => u.Username == "test_user")
                                  .FirstOrDefaultAsync();

        // Assert
        retrievedUser.Should().NotBeNull();
        retrievedUser!.Username.Should().Be("test_user");
        retrievedUser.Email.Should().Be("test@example.com");
        retrievedUser.Profile.FirstName.Should().Be("Test");
        retrievedUser.Profile.LastName.Should().BeNullOrEmpty();
        retrievedUser.Profile.Bio.Should().Be("");
        retrievedUser.Skills.Should().BeEmpty();

        _output.WriteLine("Null 值和遺失欄位處理測試通過");
    }

    [Fact]
    public async Task BsonComplexDataTypes_複雜資料型別序列化_應正確保存和讀取()
    {
        // Arrange
        var complexUser = new UserDocument
        {
            Username = "complex_user",
            Email = "complex@example.com",
            Profile = new UserProfile
            {
                FirstName = "Complex",
                LastName = "User",
                BirthDate = new DateTime(1990, 5, 15),
                Bio = "這是一個包含特殊字符的個人簡介：!@#$%^&*()_+{}|:<>?[]\\;'\",./"
            },
            Skills = new List<Skill>
            {
                new() { Name = "C#", Level = SkillLevel.Expert, YearsExperience = 10 },
                new() { Name = "MongoDB", Level = SkillLevel.Advanced, YearsExperience = 5 }
            },
            Preferences = new Dictionary<string, object>
            {
                { "theme", "dark" },
                { "language", "zh-TW" },
                { "notifications", true },
                { "max_items_per_page", 50 },
                { "tags", new[] { "developer", "mongodb", "testing" } }
            }
        };

        // Act
        await _users.InsertOneAsync(complexUser);
        var retrieved = await _users
                              .Find(u => u.Username == "complex_user")
                              .FirstOrDefaultAsync();

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Profile.Bio.Should().Be(complexUser.Profile.Bio);
        retrieved.Skills.Should().HaveCount(2);
        retrieved.Skills[0].Name.Should().Be("C#");
        retrieved.Skills[0].Level.Should().Be(SkillLevel.Expert);
        retrieved.Preferences["theme"].Should().Be("dark");
        retrieved.Preferences["notifications"].Should().Be(true);

        _output.WriteLine("複雜資料型別序列化測試通過");
    }

    [Fact]
    public async Task BsonArrayOperations_陣列操作測試_應正確更新陣列元素()
    {
        // Arrange
        var user = new UserDocument
        {
            Username = "array_test_user",
            Email = "array@example.com",
            Skills = new List<Skill>
            {
                new() { Name = "C#", Level = SkillLevel.Advanced, YearsExperience = 5 },
                new() { Name = "JavaScript", Level = SkillLevel.Intermediate, YearsExperience = 3 }
            }
        };

        await _users.InsertOneAsync(user);

        // Act - 陣列操作：新增、更新、刪除
        // 1. 新增技能
        var addSkillUpdate = Builders<UserDocument>.Update.Push("skills",
                                                                new Skill { Name = "Python", Level = SkillLevel.Beginner, YearsExperience = 1 });
        await _users.UpdateOneAsync(u => u.Username == "array_test_user", addSkillUpdate);

        // 2. 移除特定技能
        var removeSkillUpdate = Builders<UserDocument>.Update.PullFilter("skills",
                                                                         Builders<Skill>.Filter.Eq(s => s.Name, "JavaScript"));
        await _users.UpdateOneAsync(u => u.Username == "array_test_user", removeSkillUpdate);

        // 3. 更新陣列中特定元素
        var updateSkillFilter = Builders<UserDocument>.Filter.And(
            Builders<UserDocument>.Filter.Eq(u => u.Username, "array_test_user"),
            Builders<UserDocument>.Filter.ElemMatch(u => u.Skills, s => s.Name == "C#"));
        var updateSkillUpdate = Builders<UserDocument>.Update.Set("skills.$.years_experience", 7);
        await _users.UpdateOneAsync(updateSkillFilter, updateSkillUpdate);

        // Assert
        var updatedUser = await _users.Find(u => u.Username == "array_test_user").FirstOrDefaultAsync();

        updatedUser.Should().NotBeNull();
        updatedUser!.Skills.Should().HaveCount(2); // 移除了 JavaScript，新增了 Python
        updatedUser.Skills.Should().Contain(s => s.Name == "C#" && s.YearsExperience == 7);
        updatedUser.Skills.Should().Contain(s => s.Name == "Python" && s.YearsExperience == 1);
        updatedUser.Skills.Should().NotContain(s => s.Name == "JavaScript");

        _output.WriteLine($"陣列操作測試完成，最終技能數量: {updatedUser.Skills.Count}");
    }
}