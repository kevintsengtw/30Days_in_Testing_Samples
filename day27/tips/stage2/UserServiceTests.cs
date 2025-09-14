namespace Day27.Core.Tests.Services;

/// <summary>
/// 使用者服務測試類別
/// </summary>
public class UserServiceTests
{
    private readonly IUserRepository mockUserRepository;
    private readonly IEmailService mockEmailService;
    private readonly FakeTimeProvider fakeTimeProvider;
    private readonly UserService userService;

    /// <summary>
    /// 建構式：初始化測試環境
    /// </summary>
    public UserServiceTests()
    {
        mockUserRepository = Substitute.For<IUserRepository>();
        mockEmailService = Substitute.For<IEmailService>();
        fakeTimeProvider = new FakeTimeProvider();
        userService = new UserService(mockUserRepository, mockEmailService, fakeTimeProvider);
    }

    // RegisterUserAsync 註冊新使用者

    #region 正常情境測試

    [Fact(DisplayName = "註冊新使用者: 輸入有效註冊資料，應成功註冊並回傳使用者物件")]
    public async Task RegisterUserAsync_輸入有效註冊資料_應成功註冊並回傳使用者物件()
    {
        // Arrange
        var fixedTime = new DateTimeOffset(2025, 8, 14, 10, 0, 0, TimeSpan.Zero);
        fakeTimeProvider.SetUtcNow(fixedTime);
        
        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Name = "張三",
            Password = "Password123"
        };

        mockUserRepository.ExistsByEmailAsync(request.Email).Returns(false);
        mockUserRepository.SaveAsync(Arg.Any<User>()).Returns(callInfo => callInfo.Arg<User>());
        mockEmailService.SendWelcomeEmailAsync(request.Email, request.Name).Returns(true);

        // Act
        var result = await userService.RegisterUserAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(request.Email);
        result.Name.Should().Be(request.Name);
        result.Status.Should().Be(UserStatus.Active);
        result.PasswordHash.Should().NotBeNullOrEmpty();
        result.CreatedAt.Should().Be(fixedTime.DateTime);
        result.UpdatedAt.Should().Be(fixedTime.DateTime);
        
        await mockUserRepository.Received(1).ExistsByEmailAsync(request.Email);
        await mockUserRepository.Received(1).SaveAsync(Arg.Is<User>(u => 
            u.CreatedAt == fixedTime.DateTime && 
            u.UpdatedAt == fixedTime.DateTime));
        await mockEmailService.Received(1).SendWelcomeEmailAsync(request.Email, request.Name);
    }

    [Fact(DisplayName = "註冊新使用者: 輸入複雜 Email 格式，應成功註冊")]
    public async Task RegisterUserAsync_輸入複雜Email格式_應成功註冊()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Email = "user.name+tag@example.co.uk",
            Name = "李四",
            Password = "SecurePass456"
        };
        var expectedUser = new User { Id = 1, Email = request.Email, Name = request.Name, PasswordHash = "MockedPasswordHash" };

        mockUserRepository.ExistsByEmailAsync(request.Email).Returns(false);
        mockUserRepository.SaveAsync(Arg.Any<User>()).Returns(expectedUser);
        mockEmailService.SendWelcomeEmailAsync(request.Email, request.Name).Returns(true);

        // Act
        var result = await userService.RegisterUserAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(request.Email);
        await mockUserRepository.Received(1).ExistsByEmailAsync(request.Email);
    }

    [Fact(DisplayName = "註冊新使用者: 輸入包含特殊字元的姓名，應成功註冊")]
    public async Task RegisterUserAsync_輸入特殊字元姓名_應成功註冊()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Email = "test@domain.com",
            Name = "王小明-陳",
            Password = "MyPass789"
        };
        var expectedUser = new User { Id = 1, Email = request.Email, Name = request.Name, PasswordHash = "MockedPasswordHash" };

        mockUserRepository.ExistsByEmailAsync(request.Email).Returns(false);
        mockUserRepository.SaveAsync(Arg.Any<User>()).Returns(expectedUser);
        mockEmailService.SendWelcomeEmailAsync(request.Email, request.Name).Returns(true);

        // Act
        var result = await userService.RegisterUserAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
    }

    #endregion

    #region 邊界條件測試

    [Fact(DisplayName = "註冊新使用者: 輸入最短密碼長度，應成功註冊")]
    public async Task RegisterUserAsync_輸入最短密碼長度_應成功註冊()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Name = "張三",
            Password = "Pass123a"
        };
        var expectedUser = new User { Id = 1, Email = request.Email, Name = request.Name, PasswordHash = "MockedPasswordHash" };

        mockUserRepository.ExistsByEmailAsync(request.Email).Returns(false);
        mockUserRepository.SaveAsync(Arg.Any<User>()).Returns(expectedUser);
        mockEmailService.SendWelcomeEmailAsync(request.Email, request.Name).Returns(true);

        // Act
        var result = await userService.RegisterUserAsync(request);

        // Assert
        result.Should().NotBeNull();
        request.Password.Length.Should().Be(8);
    }

    [Fact(DisplayName = "註冊新使用者: 輸入最長密碼長度，應成功註冊")]
    public async Task RegisterUserAsync_輸入最長密碼長度_應成功註冊()
    {
        // Arrange
        var longPassword = "Pass123a" + new string('b', 42); // 總共 50 字元
        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Name = "張三",
            Password = longPassword
        };
        var expectedUser = new User { Id = 1, Email = request.Email, Name = request.Name, PasswordHash = "MockedPasswordHash" };

        mockUserRepository.ExistsByEmailAsync(request.Email).Returns(false);
        mockUserRepository.SaveAsync(Arg.Any<User>()).Returns(expectedUser);
        mockEmailService.SendWelcomeEmailAsync(request.Email, request.Name).Returns(true);

        // Act
        var result = await userService.RegisterUserAsync(request);

        // Assert
        result.Should().NotBeNull();
        request.Password.Length.Should().Be(50);
    }

    [Fact(DisplayName = "註冊新使用者: 輸入最短有效 Email，應成功註冊")]
    public async Task RegisterUserAsync_輸入最短有效Email_應成功註冊()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Email = "a@b.c",
            Name = "張三",
            Password = "Password123"
        };
        var expectedUser = new User { Id = 1, Email = request.Email, Name = request.Name, PasswordHash = "MockedPasswordHash" };

        mockUserRepository.ExistsByEmailAsync(request.Email).Returns(false);
        mockUserRepository.SaveAsync(Arg.Any<User>()).Returns(expectedUser);
        mockEmailService.SendWelcomeEmailAsync(request.Email, request.Name).Returns(true);

        // Act
        var result = await userService.RegisterUserAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(request.Email);
    }

    #endregion

    #region 異常處理測試

    [Fact(DisplayName = "註冊新使用者: 輸入 null 請求物件，應拋出 ArgumentNullException")]
    public async Task RegisterUserAsync_輸入Null請求物件_應拋出ArgumentNullException()
    {
        // Arrange
        RegisterUserRequest? request = null;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => userService.RegisterUserAsync(request!));
        exception.ParamName.Should().Be("request");
    }

    [Fact(DisplayName = "註冊新使用者: 輸入無效 Email 格式，應拋出 ValidationException")]
    public async Task RegisterUserAsync_輸入無效Email格式_應拋出ValidationException()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Email = "invalid-email",
            Name = "張三",
            Password = "Password123"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => userService.RegisterUserAsync(request));
        exception.Message.Should().Be("Email 格式無效");
    }

    [Fact(DisplayName = "註冊新使用者: 輸入空白 Email，應拋出 ValidationException")]
    public async Task RegisterUserAsync_輸入空白Email_應拋出ValidationException()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Email = "",
            Name = "張三",
            Password = "Password123"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => userService.RegisterUserAsync(request));
        exception.Message.Should().Be("Email 格式無效");
    }

    [Fact(DisplayName = "註冊新使用者: 輸入空白密碼，應拋出 ValidationException")]
    public async Task RegisterUserAsync_輸入空白密碼_應拋出ValidationException()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Name = "張三",
            Password = ""
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => userService.RegisterUserAsync(request));
        exception.Message.Should().Be("密碼不能為空");
    }

    [Fact(DisplayName = "註冊新使用者: 輸入密碼長度不足，應拋出 ValidationException")]
    public async Task RegisterUserAsync_輸入密碼長度不足_應拋出ValidationException()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Name = "張三",
            Password = "Pass12"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => userService.RegisterUserAsync(request));
        exception.Message.Should().Be("密碼長度至少 8 字元");
    }

    [Fact(DisplayName = "註冊新使用者: 輸入密碼長度過長，應拋出 ValidationException")]
    public async Task RegisterUserAsync_輸入密碼長度過長_應拋出ValidationException()
    {
        // Arrange
        var tooLongPassword = "Pass123" + new string('a', 45); // 總共 52 字元
        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Name = "張三",
            Password = tooLongPassword
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => userService.RegisterUserAsync(request));
        exception.Message.Should().Be("密碼長度不能超過 50 字元");
    }

    [Fact(DisplayName = "註冊新使用者: 輸入密碼缺少字母，應拋出 ValidationException")]
    public async Task RegisterUserAsync_輸入密碼缺少字母_應拋出ValidationException()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Name = "張三",
            Password = "12345678"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => userService.RegisterUserAsync(request));
        exception.Message.Should().Be("密碼必須包含字母");
    }

    [Fact(DisplayName = "註冊新使用者: 輸入密碼缺少數字，應拋出 ValidationException")]
    public async Task RegisterUserAsync_輸入密碼缺少數字_應拋出ValidationException()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Name = "張三",
            Password = "Password"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => userService.RegisterUserAsync(request));
        exception.Message.Should().Be("密碼必須包含數字");
    }

    [Fact(DisplayName = "註冊新使用者: 輸入已存在的 Email，應拋出 InvalidOperationException")]
    public async Task RegisterUserAsync_輸入已存在Email_應拋出InvalidOperationException()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Email = "existing@example.com",
            Name = "張三",
            Password = "Password123"
        };

        mockUserRepository.ExistsByEmailAsync(request.Email).Returns(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => userService.RegisterUserAsync(request));
        exception.Message.Should().Be("此 Email 已被註冊");
        
        await mockUserRepository.Received(1).ExistsByEmailAsync(request.Email);
        await mockUserRepository.DidNotReceive().SaveAsync(Arg.Any<User>());
    }

    [Fact(DisplayName = "註冊新使用者: Repository 儲存失敗時，應拋出相關異常")]
    public async Task RegisterUserAsync_Repository儲存失敗_應拋出相關異常()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Name = "張三",
            Password = "Password123"
        };
        var expectedException = new InvalidOperationException("資料庫儲存失敗");

        mockUserRepository.ExistsByEmailAsync(request.Email).Returns(false);
        mockUserRepository.SaveAsync(Arg.Any<User>()).Returns(Task.FromException<User>(expectedException));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => userService.RegisterUserAsync(request));
        exception.Message.Should().Be("資料庫儲存失敗");
        
        await mockUserRepository.Received(1).ExistsByEmailAsync(request.Email);
        await mockUserRepository.Received(1).SaveAsync(Arg.Any<User>());
        await mockEmailService.DidNotReceive().SendWelcomeEmailAsync(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact(DisplayName = "註冊新使用者: EmailService 發送失敗時，使用者仍應成功註冊")]
    public async Task RegisterUserAsync_EmailService發送失敗_使用者仍應成功註冊()
    {
        // Arrange
        var request = new RegisterUserRequest
        {
            Email = "test@example.com",
            Name = "張三",
            Password = "Password123"
        };
        var expectedUser = new User
        {
            Id = 1,
            Email = request.Email,
            Name = request.Name,
            PasswordHash = "MockedPasswordHash",
            Status = UserStatus.Active
        };

        mockUserRepository.ExistsByEmailAsync(request.Email).Returns(false);
        mockUserRepository.SaveAsync(Arg.Any<User>()).Returns(expectedUser);
        mockEmailService.SendWelcomeEmailAsync(request.Email, request.Name).Returns(Task.FromException<bool>(new InvalidOperationException("郵件服務異常")));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => userService.RegisterUserAsync(request));
        exception.Message.Should().Be("郵件服務異常");
        
        await mockUserRepository.Received(1).ExistsByEmailAsync(request.Email);
        await mockUserRepository.Received(1).SaveAsync(Arg.Any<User>());
        await mockEmailService.Received(1).SendWelcomeEmailAsync(request.Email, request.Name);
    }

    #endregion

    // ValidatePassword 驗證密碼強度

    #region ValidatePassword 正常情境測試

    [Fact(DisplayName = "驗證密碼強度: 輸入有效密碼包含字母和數字，應不拋出任何異常")]
    public void ValidatePassword_輸入有效密碼包含字母和數字_應不拋出任何異常()
    {
        // Arrange
        var password = "Password123";

        // Act & Assert
        var exception = Record.Exception(() => userService.ValidatePassword(password));
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "驗證密碼強度: 輸入混合大小寫字母和數字，應不拋出任何異常")]
    public void ValidatePassword_輸入混合大小寫字母和數字_應不拋出任何異常()
    {
        // Arrange
        var password = "MySecure123";

        // Act & Assert
        var exception = Record.Exception(() => userService.ValidatePassword(password));
        exception.Should().BeNull();
    }

    [Fact(DisplayName = "驗證密碼強度: 輸入包含特殊字元的有效密碼，應不拋出任何異常")]
    public void ValidatePassword_輸入包含特殊字元的有效密碼_應不拋出任何異常()
    {
        // Arrange
        var password = "Test123!@#";

        // Act & Assert
        var exception = Record.Exception(() => userService.ValidatePassword(password));
        exception.Should().BeNull();
    }

    #endregion

    #region ValidatePassword 邊界條件測試

    [Fact(DisplayName = "驗證密碼強度: 輸入最短有效密碼長度，應不拋出任何異常")]
    public void ValidatePassword_輸入最短有效密碼長度_應不拋出任何異常()
    {
        // Arrange
        var password = "Test123a";

        // Act & Assert
        var exception = Record.Exception(() => userService.ValidatePassword(password));
        exception.Should().BeNull();
        password.Length.Should().Be(8);
    }

    [Fact(DisplayName = "驗證密碼強度: 輸入最長有效密碼長度，應不拋出任何異常")]
    public void ValidatePassword_輸入最長有效密碼長度_應不拋出任何異常()
    {
        // Arrange
        var password = "Test123" + new string('a', 43); // 總共 50 字元

        // Act & Assert
        var exception = Record.Exception(() => userService.ValidatePassword(password));
        exception.Should().BeNull();
        password.Length.Should().Be(50);
    }

    [Fact(DisplayName = "驗證密碼強度: 輸入最少字母數字組合，應不拋出任何異常")]
    public void ValidatePassword_輸入最少字母數字組合_應不拋出任何異常()
    {
        // Arrange
        var password = "a1bcdefg";

        // Act & Assert
        var exception = Record.Exception(() => userService.ValidatePassword(password));
        exception.Should().BeNull();
    }

    #endregion

    #region ValidatePassword 異常處理測試

    [Fact(DisplayName = "驗證密碼強度: 輸入 null 密碼，應拋出 ValidationException")]
    public void ValidatePassword_輸入Null密碼_應拋出ValidationException()
    {
        // Arrange
        string? password = null;

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => userService.ValidatePassword(password!));
        exception.Message.Should().Be("密碼不能為空");
    }

    [Fact(DisplayName = "驗證密碼強度: 輸入空字串密碼，應拋出 ValidationException")]
    public void ValidatePassword_輸入空字串密碼_應拋出ValidationException()
    {
        // Arrange
        var password = "";

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => userService.ValidatePassword(password));
        exception.Message.Should().Be("密碼不能為空");
    }

    [Fact(DisplayName = "驗證密碼強度: 輸入空白字串密碼，應拋出 ValidationException")]
    public void ValidatePassword_輸入空白字串密碼_應拋出ValidationException()
    {
        // Arrange
        var password = "   ";

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => userService.ValidatePassword(password));
        exception.Message.Should().Be("密碼不能為空");
    }

    [Fact(DisplayName = "驗證密碼強度: 輸入密碼長度不足，應拋出 ValidationException")]
    public void ValidatePassword_輸入密碼長度不足_應拋出ValidationException()
    {
        // Arrange
        var password = "Test12";

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => userService.ValidatePassword(password));
        exception.Message.Should().Be("密碼長度至少 8 字元");
        password.Length.Should().BeLessThan(8);
    }

    [Fact(DisplayName = "驗證密碼強度: 輸入密碼長度過長，應拋出 ValidationException")]
    public void ValidatePassword_輸入密碼長度過長_應拋出ValidationException()
    {
        // Arrange
        var password = "Test123" + new string('a', 45); // 總共 52 字元

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => userService.ValidatePassword(password));
        exception.Message.Should().Be("密碼長度不能超過 50 字元");
        password.Length.Should().BeGreaterThan(50);
    }

    [Fact(DisplayName = "驗證密碼強度: 輸入密碼缺少字母，應拋出 ValidationException")]
    public void ValidatePassword_輸入密碼缺少字母_應拋出ValidationException()
    {
        // Arrange
        var password = "12345678";

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => userService.ValidatePassword(password));
        exception.Message.Should().Be("密碼必須包含字母");
    }

    [Fact(DisplayName = "驗證密碼強度: 輸入密碼缺少數字，應拋出 ValidationException")]
    public void ValidatePassword_輸入密碼缺少數字_應拋出ValidationException()
    {
        // Arrange
        var password = "abcdefgh";

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => userService.ValidatePassword(password));
        exception.Message.Should().Be("密碼必須包含數字");
    }

    [Fact(DisplayName = "驗證密碼強度: 輸入密碼只包含特殊字元，應拋出 ValidationException")]
    public void ValidatePassword_輸入密碼只包含特殊字元_應拋出ValidationException()
    {
        // Arrange
        var password = "!@#$%^&*";

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => userService.ValidatePassword(password));
        exception.Message.Should().Be("密碼必須包含字母");
    }

    [Fact(DisplayName = "驗證密碼強度: 輸入臨界長度且缺少要求，應拋出 ValidationException")]
    public void ValidatePassword_輸入臨界長度且缺少要求_應拋出ValidationException()
    {
        // Arrange
        var password = "1234567"; // 7 字元

        // Act & Assert
        var exception = Assert.Throws<ValidationException>(() => userService.ValidatePassword(password));
        exception.Message.Should().Be("密碼長度至少 8 字元");
        password.Length.Should().Be(7);
    }

    #endregion

    // IsValidEmail 驗證 Email 格式

    #region IsValidEmail 正常情境測試

    [Fact(DisplayName = "驗證 Email 格式: 輸入標準 Email 格式，應回傳 true")]
    public void IsValidEmail_輸入標準Email格式_應回傳True()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入包含子網域的 Email，應回傳 true")]
    public void IsValidEmail_輸入包含子網域的Email_應回傳True()
    {
        // Arrange
        var email = "user@mail.example.com";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入包含點號的用戶名，應回傳 true")]
    public void IsValidEmail_輸入包含點號的用戶名_應回傳True()
    {
        // Arrange
        var email = "first.last@example.com";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入包含加號的用戶名，應回傳 true")]
    public void IsValidEmail_輸入包含加號的用戶名_應回傳True()
    {
        // Arrange
        var email = "user+tag@example.com";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入混合大小寫，應回傳 true")]
    public void IsValidEmail_輸入混合大小寫_應回傳True()
    {
        // Arrange
        var email = "Test@Example.COM";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region IsValidEmail 邊界條件測試

    [Fact(DisplayName = "驗證 Email 格式: 輸入最短有效 Email，應回傳 true")]
    public void IsValidEmail_輸入最短有效Email_應回傳True()
    {
        // Arrange
        var email = "a@b.c";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入數字用戶名和網域，應回傳 true")]
    public void IsValidEmail_輸入數字用戶名和網域_應回傳True()
    {
        // Arrange
        var email = "123@456.789";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入特殊字元用戶名，應回傳 true")]
    public void IsValidEmail_輸入特殊字元用戶名_應回傳True()
    {
        // Arrange
        var email = "user_name@example.com";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region IsValidEmail 異常處理測試

    [Fact(DisplayName = "驗證 Email 格式: 輸入 null，應回傳 false")]
    public void IsValidEmail_輸入Null_應回傳False()
    {
        // Arrange
        string? email = null;

        // Act
        var result = userService.IsValidEmail(email!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入空字串，應回傳 false")]
    public void IsValidEmail_輸入空字串_應回傳False()
    {
        // Arrange
        var email = "";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入空白字串，應回傳 false")]
    public void IsValidEmail_輸入空白字串_應回傳False()
    {
        // Arrange
        var email = "   ";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入缺少 @ 符號，應回傳 false")]
    public void IsValidEmail_輸入缺少At符號_應回傳False()
    {
        // Arrange
        var email = "testexample.com";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入缺少網域部分，應回傳 false")]
    public void IsValidEmail_輸入缺少網域部分_應回傳False()
    {
        // Arrange
        var email = "test@";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入缺少用戶名部分，應回傳 false")]
    public void IsValidEmail_輸入缺少用戶名部分_應回傳False()
    {
        // Arrange
        var email = "@example.com";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入缺少頂級網域，應回傳 false")]
    public void IsValidEmail_輸入缺少頂級網域_應回傳False()
    {
        // Arrange
        var email = "test@example";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入多個 @ 符號，應回傳 false")]
    public void IsValidEmail_輸入多個At符號_應回傳False()
    {
        // Arrange
        var email = "test@@example.com";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入包含空白字元，應回傳 false")]
    public void IsValidEmail_輸入包含空白字元_應回傳False()
    {
        // Arrange
        var email = "test @example.com";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入以點號開始的網域，應回傳 false")]
    public void IsValidEmail_輸入以點號開始的網域_應回傳False()
    {
        // Arrange
        var email = "test@.example.com";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入以點號結束的網域，應回傳 false")]
    public void IsValidEmail_輸入以點號結束的網域_應回傳False()
    {
        // Arrange
        var email = "test@example.com.";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeFalse();
    }

    [Fact(DisplayName = "驗證 Email 格式: 輸入連續點號，應回傳 false")]
    public void IsValidEmail_輸入連續點號_應回傳False()
    {
        // Arrange
        var email = "test@example..com";

        // Act
        var result = userService.IsValidEmail(email);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    // UpdateUserStatusAsync 更新使用者狀態

    #region UpdateUserStatusAsync 正常情境測試

    [Fact(DisplayName = "更新使用者狀態: 輸入有效使用者 ID 和 Active 狀態，應成功更新並回傳使用者物件")]
    public async Task UpdateUserStatusAsync_輸入有效使用者ID和Active狀態_應成功更新並回傳使用者物件()
    {
        // Arrange
        var fixedTime = new DateTimeOffset(2025, 8, 14, 10, 0, 0, TimeSpan.Zero);
        fakeTimeProvider.SetUtcNow(fixedTime);
        
        var userId = 1;
        var newStatus = UserStatus.Active;
        var existingUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            Name = "張三",
            Status = UserStatus.Inactive,
            CreatedAt = fixedTime.AddDays(-1).DateTime,
            UpdatedAt = fixedTime.AddDays(-1).DateTime
        };

        mockUserRepository.GetByIdAsync(userId).Returns(existingUser);
        mockUserRepository.SaveAsync(Arg.Any<User>()).Returns(callInfo => callInfo.Arg<User>());

        // Act
        var result = await userService.UpdateUserStatusAsync(userId, newStatus);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(UserStatus.Active);
        result.UpdatedAt.Should().Be(fixedTime.DateTime);
        
        await mockUserRepository.Received(1).GetByIdAsync(userId);
        await mockUserRepository.Received(1).SaveAsync(Arg.Is<User>(u => u.Status == newStatus && u.UpdatedAt == fixedTime.DateTime));
    }

    [Fact(DisplayName = "更新使用者狀態: 輸入有效使用者 ID 和 Inactive 狀態，應成功更新並回傳使用者物件")]
    public async Task UpdateUserStatusAsync_輸入有效使用者ID和Inactive狀態_應成功更新並回傳使用者物件()
    {
        // Arrange
        var userId = 2;
        var newStatus = UserStatus.Inactive;
        var existingUser = new User
        {
            Id = userId,
            Email = "test2@example.com",
            Name = "李四",
            Status = UserStatus.Active
        };
        var updatedUser = new User
        {
            Id = userId,
            Email = "test2@example.com",
            Name = "李四",
            Status = UserStatus.Inactive
        };

        mockUserRepository.GetByIdAsync(userId).Returns(existingUser);
        mockUserRepository.SaveAsync(Arg.Any<User>()).Returns(updatedUser);

        // Act
        var result = await userService.UpdateUserStatusAsync(userId, newStatus);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(UserStatus.Inactive);
        
        await mockUserRepository.Received(1).GetByIdAsync(userId);
        await mockUserRepository.Received(1).SaveAsync(Arg.Any<User>());
    }

    [Fact(DisplayName = "更新使用者狀態: 輸入有效使用者 ID 和 Suspended 狀態，應成功更新並回傳使用者物件")]
    public async Task UpdateUserStatusAsync_輸入有效使用者ID和Suspended狀態_應成功更新並回傳使用者物件()
    {
        // Arrange
        var userId = 3;
        var newStatus = UserStatus.Suspended;
        var existingUser = new User
        {
            Id = userId,
            Email = "test3@example.com",
            Name = "王五",
            Status = UserStatus.Active
        };
        var updatedUser = new User
        {
            Id = userId,
            Email = "test3@example.com",
            Name = "王五",
            Status = UserStatus.Suspended
        };

        mockUserRepository.GetByIdAsync(userId).Returns(existingUser);
        mockUserRepository.SaveAsync(Arg.Any<User>()).Returns(updatedUser);

        // Act
        var result = await userService.UpdateUserStatusAsync(userId, newStatus);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(UserStatus.Suspended);
        
        await mockUserRepository.Received(1).GetByIdAsync(userId);
        await mockUserRepository.Received(1).SaveAsync(Arg.Any<User>());
    }

    #endregion

    #region UpdateUserStatusAsync 邊界條件測試

    [Fact(DisplayName = "更新使用者狀態: 輸入相同狀態值，應仍然成功更新並更新時間戳記")]
    public async Task UpdateUserStatusAsync_輸入相同狀態值_應仍然成功更新並更新時間戳記()
    {
        // Arrange
        var fixedTime = new DateTimeOffset(2025, 8, 14, 10, 0, 0, TimeSpan.Zero);
        var oldTimestamp = fixedTime.AddHours(-1);
        fakeTimeProvider.SetUtcNow(fixedTime);
        
        var userId = 1;
        var newStatus = UserStatus.Active;
        var existingUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            Name = "張三",
            Status = UserStatus.Active,
            UpdatedAt = oldTimestamp.DateTime
        };

        mockUserRepository.GetByIdAsync(userId).Returns(existingUser);
        mockUserRepository.SaveAsync(Arg.Any<User>()).Returns(callInfo => callInfo.Arg<User>());

        // Act
        var result = await userService.UpdateUserStatusAsync(userId, newStatus);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(UserStatus.Active);
        result.UpdatedAt.Should().Be(fixedTime.DateTime);
        result.UpdatedAt.Should().BeAfter(oldTimestamp.DateTime);
        
        await mockUserRepository.Received(1).GetByIdAsync(userId);
        await mockUserRepository.Received(1).SaveAsync(Arg.Any<User>());
    }

    [Fact(DisplayName = "更新使用者狀態: 輸入最小有效的使用者 ID，應成功更新")]
    public async Task UpdateUserStatusAsync_輸入最小有效的使用者ID_應成功更新()
    {
        // Arrange
        var userId = 1;
        var newStatus = UserStatus.Active;
        var existingUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            Name = "張三",
            Status = UserStatus.Inactive
        };
        var updatedUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            Name = "張三",
            Status = UserStatus.Active
        };

        mockUserRepository.GetByIdAsync(userId).Returns(existingUser);
        mockUserRepository.SaveAsync(Arg.Any<User>()).Returns(updatedUser);

        // Act
        var result = await userService.UpdateUserStatusAsync(userId, newStatus);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Status.Should().Be(UserStatus.Active);
    }

    [Fact(DisplayName = "更新使用者狀態: 輸入大數值的使用者 ID，應成功更新")]
    public async Task UpdateUserStatusAsync_輸入大數值的使用者ID_應成功更新()
    {
        // Arrange
        var userId = 999999;
        var newStatus = UserStatus.Active;
        var existingUser = new User
        {
            Id = userId,
            Email = "test999999@example.com",
            Name = "大數值使用者",
            Status = UserStatus.Inactive
        };
        var updatedUser = new User
        {
            Id = userId,
            Email = "test999999@example.com",
            Name = "大數值使用者",
            Status = UserStatus.Active
        };

        mockUserRepository.GetByIdAsync(userId).Returns(existingUser);
        mockUserRepository.SaveAsync(Arg.Any<User>()).Returns(updatedUser);

        // Act
        var result = await userService.UpdateUserStatusAsync(userId, newStatus);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(999999);
        result.Status.Should().Be(UserStatus.Active);
    }

    #endregion

    #region UpdateUserStatusAsync 異常處理測試

    [Fact(DisplayName = "更新使用者狀態: 輸入不存在的使用者 ID，應拋出 ArgumentException")]
    public async Task UpdateUserStatusAsync_輸入不存在的使用者ID_應拋出ArgumentException()
    {
        // Arrange
        var userId = 999;
        var newStatus = UserStatus.Active;

        mockUserRepository.GetByIdAsync(userId).Returns((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => userService.UpdateUserStatusAsync(userId, newStatus));
        exception.Message.Should().Be("使用者不存在 (Parameter 'userId')");
        exception.ParamName.Should().Be("userId");
        
        await mockUserRepository.Received(1).GetByIdAsync(userId);
        await mockUserRepository.DidNotReceive().SaveAsync(Arg.Any<User>());
    }

    [Fact(DisplayName = "更新使用者狀態: 輸入使用者 ID 為 0，應拋出 ArgumentException")]
    public async Task UpdateUserStatusAsync_輸入使用者ID為0_應拋出ArgumentException()
    {
        // Arrange
        var userId = 0;
        var newStatus = UserStatus.Active;

        mockUserRepository.GetByIdAsync(userId).Returns((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => userService.UpdateUserStatusAsync(userId, newStatus));
        exception.Message.Should().Be("使用者不存在 (Parameter 'userId')");
        exception.ParamName.Should().Be("userId");
        
        await mockUserRepository.Received(1).GetByIdAsync(userId);
        await mockUserRepository.DidNotReceive().SaveAsync(Arg.Any<User>());
    }

    [Fact(DisplayName = "更新使用者狀態: 輸入使用者 ID 為負數，應拋出 ArgumentException")]
    public async Task UpdateUserStatusAsync_輸入使用者ID為負數_應拋出ArgumentException()
    {
        // Arrange
        var userId = -1;
        var newStatus = UserStatus.Active;

        mockUserRepository.GetByIdAsync(userId).Returns((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => userService.UpdateUserStatusAsync(userId, newStatus));
        exception.Message.Should().Be("使用者不存在 (Parameter 'userId')");
        exception.ParamName.Should().Be("userId");
        
        await mockUserRepository.Received(1).GetByIdAsync(userId);
        await mockUserRepository.DidNotReceive().SaveAsync(Arg.Any<User>());
    }

    [Fact(DisplayName = "更新使用者狀態: Repository 查詢失敗時，應拋出相關異常")]
    public async Task UpdateUserStatusAsync_Repository查詢失敗_應拋出相關異常()
    {
        // Arrange
        var userId = 1;
        var newStatus = UserStatus.Active;
        var expectedException = new InvalidOperationException("資料庫查詢失敗");

        mockUserRepository.GetByIdAsync(userId).Returns(Task.FromException<User?>(expectedException));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => userService.UpdateUserStatusAsync(userId, newStatus));
        exception.Message.Should().Be("資料庫查詢失敗");
        
        await mockUserRepository.Received(1).GetByIdAsync(userId);
        await mockUserRepository.DidNotReceive().SaveAsync(Arg.Any<User>());
    }

    [Fact(DisplayName = "更新使用者狀態: Repository 儲存失敗時，應拋出相關異常")]
    public async Task UpdateUserStatusAsync_Repository儲存失敗_應拋出相關異常()
    {
        // Arrange
        var userId = 1;
        var newStatus = UserStatus.Active;
        var existingUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            Name = "張三",
            Status = UserStatus.Inactive
        };
        var expectedException = new InvalidOperationException("資料庫儲存失敗");

        mockUserRepository.GetByIdAsync(userId).Returns(existingUser);
        mockUserRepository.SaveAsync(Arg.Any<User>()).Returns(Task.FromException<User>(expectedException));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => userService.UpdateUserStatusAsync(userId, newStatus));
        exception.Message.Should().Be("資料庫儲存失敗");
        
        await mockUserRepository.Received(1).GetByIdAsync(userId);
        await mockUserRepository.Received(1).SaveAsync(Arg.Any<User>());
    }

    [Fact(DisplayName = "更新使用者狀態: 輸入無效的 UserStatus 枚舉值，應成功更新")]
    public async Task UpdateUserStatusAsync_輸入無效的UserStatus枚舉值_應成功更新()
    {
        // Arrange
        var userId = 1;
        var newStatus = (UserStatus)999;
        var existingUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            Name = "張三",
            Status = UserStatus.Active
        };
        var updatedUser = new User
        {
            Id = userId,
            Email = "test@example.com",
            Name = "張三",
            Status = (UserStatus)999
        };

        mockUserRepository.GetByIdAsync(userId).Returns(existingUser);
        mockUserRepository.SaveAsync(Arg.Any<User>()).Returns(updatedUser);

        // Act
        var result = await userService.UpdateUserStatusAsync(userId, newStatus);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be((UserStatus)999);
        
        await mockUserRepository.Received(1).GetByIdAsync(userId);
        await mockUserRepository.Received(1).SaveAsync(Arg.Any<User>());
    }

    #endregion
}