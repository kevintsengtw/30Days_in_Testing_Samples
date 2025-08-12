namespace Day03.Core.Tests.AdvancedTheoryTests;

/// <summary>
/// class ComplexObjectTests - 用於測試複雜物件的功能。
/// </summary>
public class ComplexObjectTests
{
    [Theory]
    [MemberData(nameof(UserScenarios))]
    public void CanAccessAdminFeatures_不同使用者角色_應回傳正確權限(User user, bool expected)
    {
        // Arrange
        var authService = new AuthorizationService();

        // Act
        var actual = authService.CanAccessAdminFeatures(user);

        // Assert
        Assert.Equal(expected, actual);
    }

    // 複雜物件的測試資料
    public static IEnumerable<object[]> UserScenarios
    {
        get
        {
            yield return
            [
                new User
                {
                    Name = "Admin User",
                    Email = "admin@company.com",
                    Roles = ["Admin", "User"],
                    Settings = new UserSettings { Theme = "Dark", Language = "zh-TW" }
                },
                true // 預期結果：可以存取管理功能
            ];

            yield return
            [
                new User
                {
                    Name = "Regular User",
                    Email = "user@company.com",
                    Roles = ["User"],
                    Settings = new UserSettings { Theme = "Light", Language = "en-US" }
                },
                false // 預期結果：無法存取管理功能
            ];
        }
    }
}