using Day03.Core.Tests.TestDataProviders;

namespace Day03.Core.Tests.BuilderPatternTests;

/// <summary>
/// class UserValidationTests - 用於測試使用者驗證的功能。
/// </summary>
public class UserValidationTests
{
    private readonly UserValidator _validator;

    /// <summary>
    /// UserValidationTests 的建構函式
    /// </summary>
    public UserValidationTests()
    {
        this._validator = new UserValidator();
    }
    
    [Theory]
    [MemberData(nameof(GetValidUsers))]
    public void ValidateUser_有效使用者_應通過驗證(User user)
    {
        // Act
        var actual = this._validator.IsValid(user);

        // Assert
        Assert.True(actual);
    }
    
    [Theory]
    [MemberData(nameof(GetInvalidUsers))]
    public void ValidateUser_無效使用者_應驗證失敗(User user)
    {
        // Act
        var actual = this._validator.IsValid(user);

        // Assert
        Assert.False(actual);
    }

    /// <summary>
    /// 取得有效的使用者測試資料
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<object[]> GetValidUsers()
    {
        var provider = new UserTestDataProvider();
        return provider.GetValidData().Select(user => new object[] { user });
    }

    /// <summary>
    /// 取得無效的使用者測試資料
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<object[]> GetInvalidUsers()
    {
        var provider = new UserTestDataProvider();
        return provider.GetInvalidData().Select(user => new object[] { user });
    }    
}