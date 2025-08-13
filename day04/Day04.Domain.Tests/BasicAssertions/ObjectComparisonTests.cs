using Day04.Domain.Models;
using Day04.Domain.Services;

namespace Day04.Domain.Tests.BasicAssertions;

/// <summary>
/// class ObjectComparisonTests - 物件比較測試
/// </summary>
public class ObjectComparisonTests
{
    private readonly UserService _userService = new();

    [Fact]
    public void BeEquivalentTo_深度物件比較_應遞迴比較所有屬性()
    {
        var expectedUser = new User
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            Profile = new UserProfile
            {
                Age = 30,
                City = "New York"
            }
        };

        var actualUser = new User
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            Profile = new UserProfile
            {
                Age = 30,
                City = "New York"
            }
        };

        // 深度物件比較
        actualUser.Should().BeEquivalentTo(expectedUser,
                                           options => options.Excluding(u => u.CreatedAt),
                                           because: "因為除了創建時間外所有屬性都應該相同");
    }

    [Fact]
    public void BeEquivalentTo_排除特定屬性_應忽略指定欄位()
    {
        var user = this._userService.CreateUser("john@example.com", "John Doe");

        var expectedUser = new User
        {
            Id = 999, // 不同的 ID
            Name = "John Doe",
            Email = "john@example.com",
            IsActive = true,
            CreatedAt = DateTime.MinValue // 不同的時間
        };

        // 排除特定屬性
        user.Should().BeEquivalentTo(expectedUser,
                                     options => options.Excluding(u => u.Id)        // 排除自動生成的 ID
                                                       .Excluding(u => u.CreatedAt) // 排除時間戳記
        );
    }
}