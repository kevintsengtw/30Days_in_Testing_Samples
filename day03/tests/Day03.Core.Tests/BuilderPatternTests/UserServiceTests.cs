namespace Day03.Core.Tests.BuilderPatternTests;

/// <summary>
/// class UserServiceTests - 用於測試使用者服務的功能。
/// </summary>
public class UserServiceTests
{
    [Fact]
    public void CreateUser_有效的管理員使用者_應成功建立()
    {
        // Arrange - 使用 Builder 模式建立測試資料
        var adminUser = UserBuilder.AnAdminUser()
                                   .WithName("John Admin")
                                   .WithEmail("john.admin@company.com")
                                   .WithAge(35)
                                   .Build();

        var userService = new UserService();

        // Act
        var actual = userService.CreateUser(adminUser);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal("John Admin", actual.Name);
        Assert.Contains("Admin", actual.Roles);
    }
    
    [Theory]
    [MemberData(nameof(GetUserScenarios))]
    public void ValidateUser_不同使用者情境_應回傳正確驗證結果(User user, bool expected)
    {
        // Arrange
        var validator = new UserValidator();

        // Act
        var actual = validator.IsValid(user);

        // Assert
        Assert.Equal(expected, actual);
    }
    
    /// <summary>
    /// 提供不同使用者情境的測試資料
    /// </summary>
    public static IEnumerable<object[]> GetUserScenarios()
    {
        // 有效使用者情境
        yield return
        [
            UserBuilder.AUser()
                       .WithName("Valid User")
                       .WithEmail("valid@example.com")
                       .WithAge(25)
                       .Build(),
            true
        ];

        // 無效使用者情境 - 空名稱
        yield return
        [
            UserBuilder.AUser()
                       .WithName("")
                       .WithEmail("valid@example.com")
                       .WithAge(25)
                       .Build(),
            false
        ];

        // 無效使用者情境 - 年齡過小
        yield return
        [
            UserBuilder.AUser()
                       .WithName("Young User")
                       .WithEmail("young@example.com")
                       .WithAge(10)
                       .Build(),
            false
        ];
    }    
}