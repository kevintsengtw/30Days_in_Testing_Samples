using Day04.Domain.Models;
using Day04.Domain.Services;

namespace Day04.Domain.Tests.PracticalExamples;

/// <summary>
/// class UserServiceTests - 使用者服務測試
/// </summary>
public class UserServiceTests
{
    private readonly UserService _userService = new();

    public UserServiceTests()
    {
        // 每個測試開始前清空資料
        this._userService.ClearUsers();
    }

    // 推薦：清楚的測試命名遵循 [方法]_[情境]_[預期結果] 模式
    [Fact]
    public void CreateUser_有效使用者資料_應回傳啟用的使用者()
    {
        // Arrange
        var userData = new CreateUserRequest
        {
            Email = "test@example.com",
            Name = "Test User"
        };

        // Act
        var result = this._userService.CreateUser(userData);

        // Assert
        result.Should().NotBeNull()
              .And.BeOfType<User>();
        result.Email.Should().Be(userData.Email);
        result.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "姓名不能為空")]
    [InlineData(null, "姓名不能為空")]
    [InlineData("A", "姓名至少需要2個字元")]
    public void CreateUser_無效使用者姓名_應拋出ArgumentException(string? invalidName, string expectedError)
    {
        // Arrange
        var userData = new CreateUserRequest
        {
            Email = "test@example.com",
            Name = invalidName ?? string.Empty
        };

        // Act
        Action action = () => this._userService.CreateUser(userData);

        // Assert
        action.Should().Throw<ArgumentException>()
              .WithMessage($"{expectedError}*");
    }

    [Fact]
    public void GetActiveUsers_多個使用者_應只回傳啟用的使用者()
    {
        // Arrange
        var user1 = this._userService.CreateUser("user1@example.com", "User One");
        var user2 = this._userService.CreateUser("user2@example.com", "User Two");

        // 模擬停用一個使用者
        user2.IsActive = false;

        // Act
        var activeUsers = this._userService.GetActiveUsers();

        // Assert
        activeUsers.Should().HaveCount(1)
                   .And.Contain(u => u.Id == user1.Id)
                   .And.NotContain(u => u.Id == user2.Id);
    }
}