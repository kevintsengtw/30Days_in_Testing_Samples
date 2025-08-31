using Day22.Core.Models.Mongo;
using Day22.Integration.Tests.Fixtures;
using Microsoft.Extensions.Time.Testing;

namespace Day22.Integration.Tests.MongoDB;

/// <summary>
/// MongoDB 使用者服務整合測試
/// </summary>
[Collection("MongoDb Collection")]
public class MongoUserServiceTests
{
    private readonly MongoUserService _mongoUserService;
    private readonly IMongoDatabase _database;
    private readonly FakeTimeProvider _fakeTimeProvider;
    private readonly MongoDbContainerFixture _fixture;

    public MongoUserServiceTests(MongoDbContainerFixture fixture)
    {
        _fixture = fixture;
        _database = fixture.Database;
        var settings = TestSettings.CreateMongoDbSettings();
        var logger = TestSettings.CreateLogger<MongoUserService>();
        _fakeTimeProvider = TestSettings.CreateFakeTimeProvider();
        _mongoUserService = new MongoUserService(_database, settings, logger, _fakeTimeProvider);
    }

    [Fact]
    public async Task CreateUserAsync_輸入有效使用者_應成功建立使用者()
    {
        // Arrange
        var user = new UserDocument
        {
            Username = "testuser",
            Email = "test@example.com",
            Profile = new UserProfile
            {
                FirstName = "Test",
                LastName = "User",
                Bio = "Test user bio"
            }
        };

        // Act
        var result = await _mongoUserService.CreateUserAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be("testuser");
        result.Email.Should().Be("test@example.com");
        result.Id.Should().NotBeEmpty();
        result.CreatedAt.Should().Be(_fakeTimeProvider.GetUtcNow().DateTime);
    }

    [Fact]
    public async Task GetUserByIdAsync_輸入存在的ID_應回傳正確使用者()
    {
        // Arrange
        var user = new UserDocument
        {
            Username = "gettest",
            Email = "gettest@example.com",
            Profile = new UserProfile { FirstName = "Get", LastName = "Test" }
        };
        var createdUser = await _mongoUserService.CreateUserAsync(user);

        // Act
        var result = await _mongoUserService.GetUserByIdAsync(createdUser.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("gettest");
        result.Email.Should().Be("gettest@example.com");
    }

    [Fact]
    public async Task GetUserByEmailAsync_輸入存在的Email_應回傳正確使用者()
    {
        // Arrange
        var user = new UserDocument
        {
            Username = "emailtest",
            Email = "emailtest@example.com",
            Profile = new UserProfile { FirstName = "Email", LastName = "Test" }
        };
        await _mongoUserService.CreateUserAsync(user);

        // Act
        var result = await _mongoUserService.GetUserByEmailAsync("emailtest@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be("emailtest");
        result.Email.Should().Be("emailtest@example.com");
    }

    [Fact]
    public async Task UpdateUserAsync_輸入修改的使用者_應成功更新()
    {
        // Arrange
        var user = new UserDocument
        {
            Username = "updatetest",
            Email = "updatetest@example.com",
            Profile = new UserProfile { FirstName = "Update", LastName = "Test" }
        };
        var createdUser = await _mongoUserService.CreateUserAsync(user);

        // 修改使用者資料
        createdUser.Profile.Bio = "Updated bio";

        // Act
        var result = await _mongoUserService.UpdateUserAsync(createdUser);

        // Assert
        result.Should().NotBeNull();
        result!.Profile.Bio.Should().Be("Updated bio");
        result.Version.Should().Be(2); // 版本應該增加
    }

    [Fact]
    public async Task DeleteUserAsync_輸入存在的ID_應成功刪除()
    {
        // Arrange
        var user = new UserDocument
        {
            Username = "deletetest",
            Email = "deletetest@example.com",
            Profile = new UserProfile { FirstName = "Delete", LastName = "Test" }
        };
        var createdUser = await _mongoUserService.CreateUserAsync(user);

        // Act
        var result = await _mongoUserService.DeleteUserAsync(createdUser.Id);

        // Assert
        result.Should().BeTrue();

        // 驗證使用者已被刪除
        var deletedUser = await _mongoUserService.GetUserByIdAsync(createdUser.Id);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task CreateUsersAsync_輸入多個使用者_應成功批次建立()
    {
        // Arrange
        var users = new[]
        {
            new UserDocument { Username = "batch1", Email = "batch1@example.com", Profile = new UserProfile { FirstName = "Batch", LastName = "One" } },
            new UserDocument { Username = "batch2", Email = "batch2@example.com", Profile = new UserProfile { FirstName = "Batch", LastName = "Two" } },
            new UserDocument { Username = "batch3", Email = "batch3@example.com", Profile = new UserProfile { FirstName = "Batch", LastName = "Three" } }
        };

        // Act
        var result = await _mongoUserService.CreateUsersAsync(users);

        // Assert
        result.Should().Be(3);

        // 驗證使用者已建立
        var allUsers = await _mongoUserService.GetAllUsersAsync();
        allUsers.Count(u => u.Username.StartsWith("batch")).Should().Be(3);
    }

    [Fact]
    public async Task UserExistsAsync_輸入存在的Email_應回傳True()
    {
        // Arrange
        var user = new UserDocument
        {
            Username = "existstest",
            Email = "existstest@example.com",
            Profile = new UserProfile { FirstName = "Exists", LastName = "Test" }
        };
        await _mongoUserService.CreateUserAsync(user);

        // Act
        var result = await _mongoUserService.UserExistsAsync("existstest@example.com");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task UserExistsAsync_輸入不存在的Email_應回傳False()
    {
        // Arrange & Act
        var result = await _mongoUserService.UserExistsAsync("nonexistent@example.com");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddUserSkillAsync_輸入新技能_應成功新增()
    {
        // Arrange
        var user = new UserDocument
        {
            Username = "skilltest",
            Email = "skilltest@example.com",
            Profile = new UserProfile { FirstName = "Skill", LastName = "Test" }
        };
        var createdUser = await _mongoUserService.CreateUserAsync(user);

        var skill = new Skill
        {
            Name = "C#",
            Level = SkillLevel.Advanced,
            YearsExperience = 5,
            Verified = true
        };

        // Act
        var result = await _mongoUserService.AddUserSkillAsync(createdUser.Id, skill);

        // Assert
        result.Should().BeTrue();

        // 驗證技能已新增
        var updatedUser = await _mongoUserService.GetUserByIdAsync(createdUser.Id);
        updatedUser!.Skills.Should().ContainSingle(s => s.Name == "C#");
        updatedUser.Skills.First(s => s.Name == "C#").Level.Should().Be(SkillLevel.Advanced);
    }

    [Fact]
    public async Task UpdateUserSkillLevelAsync_輸入現有技能_應成功更新等級()
    {
        // Arrange
        var user = new UserDocument
        {
            Username = "skillupdatetest",
            Email = "skillupdatetest@example.com",
            Profile = new UserProfile { FirstName = "SkillUpdate", LastName = "Test" },
            Skills = new List<Skill>
            {
                new Skill { Name = "Python", Level = SkillLevel.Beginner, YearsExperience = 1 }
            }
        };
        var createdUser = await _mongoUserService.CreateUserAsync(user);

        // Act
        var result = await _mongoUserService.UpdateUserSkillLevelAsync(createdUser.Id, "Python", SkillLevel.Intermediate);

        // Assert
        result.Should().BeTrue();

        // 驗證技能等級已更新
        var updatedUser = await _mongoUserService.GetUserByIdAsync(createdUser.Id);
        updatedUser!.Skills.First(s => s.Name == "Python").Level.Should().Be(SkillLevel.Intermediate);
    }

    [Fact]
    public async Task AddUserAddressAsync_輸入新地址_應成功新增()
    {
        // Arrange
        var user = new UserDocument
        {
            Username = "addresstest",
            Email = "addresstest@example.com",
            Profile = new UserProfile { FirstName = "Address", LastName = "Test" }
        };
        var createdUser = await _mongoUserService.CreateUserAsync(user);

        var address = new Address
        {
            Type = "home",
            Street = "123 Test Street",
            City = "Test City",
            State = "Test State",
            Country = "Test Country",
            PostalCode = "12345",
            IsPrimary = true,
            Location = GeoLocation.CreatePoint(121.5654, 25.0330) // 台北市經緯度
        };

        // Act
        var result = await _mongoUserService.AddUserAddressAsync(createdUser.Id, address);

        // Assert
        result.Should().BeTrue();

        // 驗證地址已新增
        var updatedUser = await _mongoUserService.GetUserByIdAsync(createdUser.Id);
        updatedUser!.Addresses.Should().ContainSingle();
        updatedUser.Addresses.First().Street.Should().Be("123 Test Street");
        Math.Abs(updatedUser.Addresses.First().Location!.Longitude - 121.5654).Should().BeLessThan(0.0001);
    }

    [Fact]
    public async Task SearchUsersAsync_輸入搜尋文字_應回傳符合的使用者()
    {
        // Arrange
        var user1 = new UserDocument
        {
            Username = "searcher1",
            Email = "search1@example.com",
            Profile = new UserProfile { FirstName = "Search", LastName = "One", Bio = "Loves programming" }
        };
        var user2 = new UserDocument
        {
            Username = "searcher2",
            Email = "search2@example.com",
            Profile = new UserProfile { FirstName = "Search", LastName = "Two", Bio = "Enjoys testing" }
        };

        await _mongoUserService.CreateUserAsync(user1);
        await _mongoUserService.CreateUserAsync(user2);

        // Act
        var result = await _mongoUserService.SearchUsersAsync("programming");

        // Assert
        result.Should().ContainSingle();
        result.First().Profile.Bio.Should().Contain("programming");
    }

    [Fact]
    public async Task DeactivateUserAsync_輸入使用者ID_應成功停用使用者()
    {
        // Arrange
        var user = new UserDocument
        {
            Username = "deactivatetest",
            Email = "deactivatetest@example.com",
            Profile = new UserProfile { FirstName = "Deactivate", LastName = "Test" },
            IsActive = true
        };
        var createdUser = await _mongoUserService.CreateUserAsync(user);

        // Act
        var result = await _mongoUserService.DeactivateUserAsync(createdUser.Id);

        // Assert
        result.Should().BeTrue();

        // 驗證使用者已停用
        var updatedUser = await _mongoUserService.GetUserByIdAsync(createdUser.Id);
        updatedUser!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task ActivateUserAsync_輸入停用的使用者ID_應成功啟用使用者()
    {
        // Arrange
        var user = new UserDocument
        {
            Username = "activatetest",
            Email = "activatetest@example.com",
            Profile = new UserProfile { FirstName = "Activate", LastName = "Test" },
            IsActive = false
        };
        var createdUser = await _mongoUserService.CreateUserAsync(user);

        // Act
        var result = await _mongoUserService.ActivateUserAsync(createdUser.Id);

        // Assert
        result.Should().BeTrue();

        // 驗證使用者已啟用
        var updatedUser = await _mongoUserService.GetUserByIdAsync(createdUser.Id);
        updatedUser!.IsActive.Should().BeTrue();
    }
}