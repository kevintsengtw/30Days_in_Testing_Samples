namespace TUnit.Demo.Tests;

/// <summary>
/// EmailValidator 類別的測試案例
/// </summary>
public class EmailValidatorTests
{
    private readonly EmailValidator _validator;

    public EmailValidatorTests()
    {
        _validator = new EmailValidator();
    }

    [Test]
    [Arguments("test@example.com", true)]
    [Arguments("user@domain.org", true)]
    [Arguments("invalid-email", false)]
    [Arguments("", false)]
    [Arguments(null, false)]
    [Arguments("@example.com", false)]
    [Arguments("test@", false)]
    public async Task IsValidEmail_各種輸入_應回傳正確驗證結果(string? email, bool expected)
    {
        // Act
        var result = _validator.IsValidEmail(email);

        // Assert
        await Assert.That(result).IsEqualTo(expected);
    }

    [Test]
    public async Task GetDomain_輸入有效Email_應回傳正確網域()
    {
        // Arrange
        var email = "user@example.com";
        var expectedDomain = "example.com";

        // Act
        var result = _validator.GetDomain(email);

        // Assert
        await Assert.That(result).IsEqualTo(expectedDomain);
    }

    [Test]
    public async Task GetDomain_輸入無效Email_應拋出ArgumentException()
    {
        // Arrange
        var invalidEmail = "invalid-email";

        // Act & Assert
        await Assert.That(() => _validator.GetDomain(invalidEmail))
                    .Throws<ArgumentException>();
    }
}