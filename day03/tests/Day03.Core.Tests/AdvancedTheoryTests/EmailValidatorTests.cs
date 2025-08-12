namespace Day03.Core.Tests.AdvancedTheoryTests;

/// <summary>
/// class EmailValidatorTests - 用於測試字串驗證的功能。
/// </summary>
public class EmailValidatorTests
{
    // 使用靜態方法動態產生測試資料
    public static IEnumerable<object[]> GetEmailTestData()
    {
        // 有效的 Email 格式
        yield return ["test@example.com", true];
        yield return ["user.name@company.co.uk", true];
        yield return ["admin+tag@service.org", true];

        // 無效的 Email 格式
        yield return ["invalid-email", false];
        yield return ["@example.com", false];
        yield return ["test@", false];
        yield return ["", false];
        yield return [null!, false];

        // 邊界值測試
        yield return [new string('a', 64) + "@example.com", false]; // 超過最大長度
    }
    
    [Theory]
    [MemberData(nameof(GetEmailTestData))]
    public void IsValidEmail_輸入各種格式的Email_應回傳預期驗證結果(string email, bool expected)
    {
        // Arrange
        var validator = new EmailValidator();

        // Act
        var actual = validator.IsValidEmail(email);

        // Assert
        Assert.Equal(expected, actual);
    }    
}