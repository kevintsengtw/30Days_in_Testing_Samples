namespace Day18.Core.Tests.Validators;

public class UserRegistrationValidatorTests
{
    private readonly FakeTimeProvider _fakeTimeProvider;
    private readonly UserRegistrationValidator _validator;

    public UserRegistrationValidatorTests()
    {
        // 根據 Microsoft 文件設定固定的測試時間 (2024/1/1)
        _fakeTimeProvider = new FakeTimeProvider();
        _fakeTimeProvider.SetUtcNow(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
        _validator = new UserRegistrationValidator(_fakeTimeProvider);
    }

    [Theory]
    [InlineData("", "使用者名稱不可為 null 或空白")]
    [InlineData(null, "使用者名稱不可為 null 或空白")]
    [InlineData("a", "使用者名稱長度必須在 3 到 20 個字元之間")]
    [InlineData("ab", "使用者名稱長度必須在 3 到 20 個字元之間")]
    [InlineData("this_is_a_very_long_username_that_exceeds_the_limit", "使用者名稱長度必須在 3 到 20 個字元之間")]
    [InlineData("user@name", "使用者名稱只能包含字母、數字和底線")]
    [InlineData("user name", "使用者名稱只能包含字母、數字和底線")]
    public void Validate_無效的使用者名稱_應該回傳對應錯誤訊息(string? username, string expectedErrorMessage)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Username = username!;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username)
              .WithErrorMessage(expectedErrorMessage);
    }

    [Theory]
    [InlineData("validuser123")]
    [InlineData("user_name")]
    [InlineData("User123")]
    [InlineData("test_user_456")]
    [InlineData("user123")]
    public void Validate_有效的使用者名稱_應該通過驗證(string username)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Username = username;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }

    [Theory]
    [InlineData("", "電子郵件不可為 null 或空白")]
    [InlineData("invalid", "電子郵件格式不正確")]
    [InlineData("invalid@", "電子郵件格式不正確")]
    [InlineData("@example.com", "電子郵件格式不正確")]
    public void Validate_無效的電子郵件_應該驗證失敗(string email, string expectedErrorMessage)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Email = email;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage(expectedErrorMessage);
    }

    [Fact]
    public void Validate_過長的電子郵件_應該驗證失敗()
    {
        // Arrange
        var request = CreateValidRequest();
        // 產生一個確實超過 100 個字元的電子郵件
        var longEmail = new string('a', 85) + "@example.com"; // 85 + 1 + 11 = 97，需要更長
        request.Email = new string('a', 90) + "@test.com";    // 90 + 1 + 8 = 99，還是不夠
        request.Email = new string('a', 92) + "@test.com";    // 92 + 1 + 8 = 101，這樣就超過 100 了

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("電子郵件長度不能超過 100 個字元");
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("firstname+lastname@company.org")]
    public void Validate_有效的電子郵件_應該通過驗證(string email)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Email = email;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    [Theory]
    [InlineData("weak", "密碼長度必須在 8 到 50 個字元之間")]
    [InlineData("weakpass", "密碼必須包含大小寫字母和數字")]
    [InlineData("WEAKPASS123", "密碼必須包含大小寫字母和數字")]
    [InlineData("weakpass123", "密碼必須包含大小寫字母和數字")]
    [InlineData("WeakPass", "密碼必須包含大小寫字母和數字")]
    public void Validate_不符合複雜度要求的密碼_應該驗證失敗(string password, string expectedErrorMessage)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Password = password;
        request.ConfirmPassword = password;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password)
              .WithErrorMessage(expectedErrorMessage);
    }

    [Theory]
    [InlineData("Password123")]
    [InlineData("MySecure1")]
    [InlineData("StrongPass99")]
    public void Validate_符合複雜度要求的密碼_應該通過驗證(string password)
    {
        // Arrange
        var request = CreateValidRequest();
        request.Password = password;
        request.ConfirmPassword = password;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_密碼與確認密碼不一致_應該驗證失敗()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Password = "Password123";
        request.ConfirmPassword = "DifferentPass456";

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword)
              .WithErrorMessage("確認密碼必須與密碼相同");
    }
    
    [Fact]
    public void Validate_年齡低於18歲_應該驗證失敗()
    {
        // Arrange
        // 設定測試當下的時間為 2024/1/1
        _fakeTimeProvider.SetUtcNow(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var request = CreateValidRequest();
        request.Age = 17;
        request.BirthDate = new DateTime(2006, 6, 1); // 在 2024/1/1 時還未滿 18 歲

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Age)
              .WithErrorMessage("年齡必須大於或等於 18 歲");
    }

    [Fact]
    public void Validate_年齡超過120歲_應該驗證失敗()
    {
        // Arrange
        // 設定測試當下的時間為 2024/1/1
        _fakeTimeProvider.SetUtcNow(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var request = CreateValidRequest();
        request.Age = 121;
        request.BirthDate = new DateTime(1902, 1, 1); // 121 歲

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Age)
              .WithErrorMessage("年齡必須小於或等於 120 歲");
    }

    [Theory]
    [InlineData(18)]  // 邊界下限
    [InlineData(120)] // 邊界上限
    [InlineData(25)]  // 一般情況
    public void Validate_有效年齡_應該通過驗證(int age)
    {
        // Arrange
        // 設定測試當下的時間為 2024/1/1
        _fakeTimeProvider.SetUtcNow(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

        var request = CreateValidRequest();
        request.Age = age;
        request.BirthDate = new DateTime(2024 - age, 1, 1); // 確保年齡與生日一致

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Age);
    }

    [Fact]
    public void Validate_年齡與出生日期不一致_應該驗證失敗()
    {
        // Arrange
        // 設定測試當下的時間為 2024/5/12
        _fakeTimeProvider.SetUtcNow(new DateTimeOffset(2024, 5, 12, 0, 0, 0, TimeSpan.Zero));

        var request = CreateValidRequest();
        request.BirthDate = new DateTime(2000, 1, 1); // 出生日期顯示應該是 24 歲
        request.Age = 29;                             // 但是設定年齡為 29 歲，不一致

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.BirthDate)
              .WithErrorMessage("生日與年齡不一致");
    }

    [Fact]
    public void Validate_年齡與出生日期一致且滿18歲_應該通過驗證()
    {
        // Arrange
        // 設定測試當下的時間為 2024/5/12
        _fakeTimeProvider.SetUtcNow(new DateTimeOffset(2024, 5, 12, 0, 0, 0, TimeSpan.Zero));

        var request = CreateValidRequest();
        request.BirthDate = new DateTime(2000, 1, 1); // 出生日期 
        request.Age = 24;                             // 在 2024/5/12 時正確的年齡

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.BirthDate);
        result.ShouldNotHaveValidationErrorFor(x => x.Age);
    }

    [Fact]
    public void Validate_未同意服務條款_應該驗證失敗()
    {
        // Arrange
        var request = CreateValidRequest();
        request.AgreeToTerms = false;

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AgreeToTerms)
              .WithErrorMessage("必須同意使用條款");
    }

    [Fact]
    public void Validate_完整的有效請求_應該通過驗證()
    {
        // Arrange
        var request = CreateValidRequest();

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_多個無效欄位_應該回傳所有錯誤()
    {
        // Arrange
        var request = CreateValidRequest();
        request.Username = "";        // 無效使用者名稱
        request.Email = "invalid";    // 無效電子郵件
        request.Password = "weak";    // 無效密碼
        request.AgreeToTerms = false; // 未同意條款

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.Password);
        result.ShouldHaveValidationErrorFor(x => x.AgreeToTerms);
    }

    private UserRegistrationRequest CreateValidRequest()
    {
        // 使用 FakeTimeProvider 的時間來計算年齡 (2024/1/1)
        var currentYear = _fakeTimeProvider.GetLocalNow().Year;
        return new UserRegistrationRequest
        {
            Username = "testuser123",
            Email = "test@example.com",
            Password = "TestPass123",
            ConfirmPassword = "TestPass123",
            BirthDate = new DateTime(1990, 1, 1),
            Age = currentYear - 1990, // 基於 FakeTimeProvider 計算年齡 (2024 - 1990 = 34)
            PhoneNumber = "0912345678",
            Roles = ["User"],
            AgreeToTerms = true
        };
    }
}