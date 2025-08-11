using Day01.Core;

namespace Day01.Samples.Tests;

/// <summary>
/// class EmailHelperTests - Email 測試類別
/// </summary>
public class EmailHelperTests
{
    private readonly EmailHelper _emailHelper;

    public EmailHelperTests()
    {
        this._emailHelper = new EmailHelper();
    }

    //---------------------------------------------------------------------------------------------
    // IsValidEmail 方法測試

    [Fact] // Fast: 不依賴外部資源
    public void IsValidEmail_輸入有效Email_應回傳True()
    {
        // Arrange
        const string email = "test@example.com";

        // Act
        var result = this._emailHelper.IsValidEmail(email);

        // Assert
        Assert.True(result); // Self-Validating
    }

    [Fact] // Independent: 不依賴其他測試
    public void IsValidEmail_輸入null值_應回傳False()
    {
        // Arrange
        string? email = null;

        // Act
        var result = this._emailHelper.IsValidEmail(email);

        // Assert
        Assert.False(result);
    }

    [Fact] // Repeatable: 每次執行結果都一樣
    public void IsValidEmail_輸入空字串_應回傳False()
    {
        // Arrange
        const string email = "";

        // Act
        var result = this._emailHelper.IsValidEmail(email);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidEmail_輸入只有空白字元_應回傳False()
    {
        // Arrange
        const string email = "   ";

        // Act
        var result = this._emailHelper.IsValidEmail(email);

        // Assert
        Assert.False(result);
    }

    //---------------------------------------------------------------------------------------------
    // Theory 測試 - 測試多個無效格式

    [Theory] // 使用 xUnit Theory 測試多個案例
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test.example.com")]
    [InlineData("test@.com")]
    [InlineData("test@com")]
    [InlineData("test@@example.com")]
    public void IsValidEmail_輸入無效Email格式_應回傳False(string invalidEmail)
    {
        // Act
        var result = this._emailHelper.IsValidEmail(invalidEmail);

        // Assert
        Assert.False(result);
    }

    [Theory] // 使用 Theory 測試多個有效案例
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.org")]
    [InlineData("admin@company.co.uk")]
    [InlineData("test123@test-domain.com")]
    [InlineData("user+tag@example.com")]
    public void IsValidEmail_輸入有效Email格式_應回傳True(string validEmail)
    {
        // Act
        var result = this._emailHelper.IsValidEmail(validEmail);

        // Assert
        Assert.True(result);
    }

    // ---------------------------------------------------------------------------------------------
    // GetDomain 方法測試

    [Fact]
    public void GetDomain_輸入有效Email_應回傳網域名稱()
    {
        // Arrange
        const string email = "user@example.com";
        const string expectedDomain = "example.com";

        // Act
        var result = this._emailHelper.GetDomain(email);

        // Assert
        Assert.Equal(expectedDomain, result);
    }

    [Fact]
    public void GetDomain_輸入無效Email_應回傳null()
    {
        // Arrange
        const string email = "invalid-email";

        // Act
        var result = this._emailHelper.GetDomain(email);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetDomain_輸入null_應回傳null()
    {
        // Arrange
        string? email = null;

        // Act
        var result = this._emailHelper.GetDomain(email);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("test@gmail.com", "gmail.com")]
    [InlineData("admin@company.co.uk", "company.co.uk")]
    [InlineData("user@sub.domain.org", "sub.domain.org")]
    public void GetDomain_輸入各種有效Email_應回傳對應網域(string email, string expectedDomain)
    {
        // Act
        var result = this._emailHelper.GetDomain(email);

        // Assert
        Assert.Equal(expectedDomain, result);
    }
}