namespace Day18.Core.Tests.Validators
{
    public class UserRegistrationValidatorTests
    {
        private readonly FakeTimeProvider _fakeTimeProvider;
        private readonly UserRegistrationValidator _validator;

        public UserRegistrationValidatorTests()
        {
            // 根據 Microsoft 文件設定固定的測試時間 (2024/1/1)
            this._fakeTimeProvider = new FakeTimeProvider();
            this._fakeTimeProvider.SetUtcNow(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));
            this._validator = new UserRegistrationValidator(this._fakeTimeProvider);
        }

        [Fact]
        public void Validate_有效的使用者名稱_應該通過驗證()
        {
            // Arrange
            var request = this.CreateValidRequest();
            request.Username = "validuser123";

            // Act
            var result = this._validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Username);
        }

        [Fact]
        public void Validate_空的使用者名稱_應該驗證失敗()
        {
            // Arrange
            var request = this.CreateValidRequest();
            request.Username = "";

            // Act
            var result = this._validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Username)
                  .WithErrorMessage("使用者名稱不可為 null 或空白");
        }

        [Theory]
        [InlineData("", "使用者名稱不可為 null 或空白")]
        [InlineData("a", "使用者名稱長度必須在 3 到 20 個字元之間")]
        [InlineData("ab", "使用者名稱長度必須在 3 到 20 個字元之間")]
        [InlineData("user@name", "使用者名稱只能包含字母、數字和底線")]
        public void Validate_無效的使用者名稱_應該回傳對應錯誤訊息(string username, string expectedErrorMessage)
        {
            // Arrange
            var request = this.CreateValidRequest();
            request.Username = username;

            // Act
            var result = this._validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Username)
                  .WithErrorMessage(expectedErrorMessage);
        }

        [Theory]
        [InlineData("validuser123")]
        [InlineData("user_name")]
        [InlineData("User123")]
        public void Validate_各種有效的使用者名稱格式_應該通過驗證(string username)
        {
            // Arrange
            var request = this.CreateValidRequest();
            request.Username = username;

            // Act
            var result = this._validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Username);
        }

        [Fact]
        public void Validate_無效的電子郵件_應該驗證失敗()
        {
            // Arrange
            var request = this.CreateValidRequest();
            request.Email = "invalid";

            // Act
            var result = this._validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("電子郵件格式不正確");
        }

        [Fact]
        public void Validate_密碼太短_應該驗證失敗()
        {
            // Arrange
            var request = this.CreateValidRequest();
            request.Password = "weak";
            request.ConfirmPassword = "weak";

            // Act
            var result = this._validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("密碼長度必須在 8 到 50 個字元之間");
        }

        [Fact]
        public void Validate_年齡未滿18歲_應該驗證失敗()
        {
            // Arrange
            // 設定測試當下的時間為 2024/1/1
            this._fakeTimeProvider.SetUtcNow(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

            var request = this.CreateValidRequest();
            request.Age = 17;
            request.BirthDate = new DateTime(2006, 6, 1); // 在 2024/1/1 時還未滿 18 歲

            // Act
            var result = this._validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Age)
                  .WithErrorMessage("年齡必須大於或等於 18 歲");
        }

        [Fact]
        public void Validate_年齡與出生日期不一致_應該驗證失敗()
        {
            // Arrange
            // 設定測試當下的時間為 2024/5/12
            this._fakeTimeProvider.SetUtcNow(new DateTimeOffset(2024, 5, 12, 0, 0, 0, TimeSpan.Zero));

            var request = this.CreateValidRequest();
            request.BirthDate = new DateTime(2000, 1, 1); // 出生日期顯示應該是 24 歲
            request.Age = 29;                             // 但是設定年齡為 29 歲，不一致

            // Act
            var result = this._validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.BirthDate)
                  .WithErrorMessage("生日與年齡不一致");
        }

        [Fact]
        public void Validate_年齡與出生日期一致且滿18歲_應該通過驗證()
        {
            // Arrange
            // 設定測試當下的時間為 2024/5/12
            this._fakeTimeProvider.SetUtcNow(new DateTimeOffset(2024, 5, 12, 0, 0, 0, TimeSpan.Zero));

            var request = this.CreateValidRequest();
            request.BirthDate = new DateTime(2000, 1, 1); // 出生日期 
            request.Age = 24;                             // 在 2024/5/12 時正確的年齡

            // Act
            var result = this._validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.BirthDate);
            result.ShouldNotHaveValidationErrorFor(x => x.Age);
        }

        [Fact]
        public void Validate_未同意服務條款_應該驗證失敗()
        {
            // Arrange
            var request = this.CreateValidRequest();
            request.AgreeToTerms = false;

            // Act
            var result = this._validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.AgreeToTerms)
                  .WithErrorMessage("必須同意使用條款");
        }

        [Fact]
        public void Validate_完整的有效請求_應該通過驗證()
        {
            // Arrange
            var request = this.CreateValidRequest();

            // Act
            var result = this._validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_多個無效欄位_應該回傳所有錯誤()
        {
            // Arrange
            var request = this.CreateValidRequest();
            request.Username = "";        // 無效使用者名稱
            request.Email = "invalid";    // 無效電子郵件
            request.Password = "weak";    // 無效密碼
            request.AgreeToTerms = false; // 未同意條款

            // Act
            var result = this._validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Username);
            result.ShouldHaveValidationErrorFor(x => x.Email);
            result.ShouldHaveValidationErrorFor(x => x.Password);
            result.ShouldHaveValidationErrorFor(x => x.AgreeToTerms);
        }

        private UserRegistrationRequest CreateValidRequest()
        {
            // 使用 FakeTimeProvider 的時間來計算年齡 (2024/1/1)
            var currentYear = this._fakeTimeProvider.GetLocalNow().Year;
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
}