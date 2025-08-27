namespace Day18.Core.Tests.Validators
{
    public class UserRegistrationAsyncValidatorTests
    {
        private readonly IUserService _mockUserService;
        private readonly FakeTimeProvider _fakeTimeProvider;
        private readonly UserRegistrationAsyncValidator _validator;

        public UserRegistrationAsyncValidatorTests()
        {
            this._mockUserService = Substitute.For<IUserService>();
            this._fakeTimeProvider = new FakeTimeProvider();
            this._fakeTimeProvider.SetUtcNow(new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero));

            this._validator = new UserRegistrationAsyncValidator(this._mockUserService, this._fakeTimeProvider);
        }

        [Fact]
        public async Task ValidateAsync_使用者名稱可用_應該通過驗證()
        {
            // Arrange
            var request = this.CreateValidRequest();
            request.Username = "newuser123";

            this._mockUserService.IsUsernameAvailableAsync("newuser123")
                .Returns(Task.FromResult(true));
            this._mockUserService.IsEmailRegisteredAsync(Arg.Any<string>())
                .Returns(Task.FromResult(false));

            // Act
            var result = await this._validator.TestValidateAsync(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Username);
            await this._mockUserService.Received(1).IsUsernameAvailableAsync("newuser123");
        }

        [Fact]
        public async Task ValidateAsync_使用者名稱已被使用_應該驗證失敗()
        {
            // Arrange
            var request = this.CreateValidRequest();
            request.Username = "existinguser";

            this._mockUserService.IsUsernameAvailableAsync("existinguser")
                .Returns(Task.FromResult(false));
            this._mockUserService.IsEmailRegisteredAsync(Arg.Any<string>())
                .Returns(Task.FromResult(false));

            // Act
            var result = await this._validator.TestValidateAsync(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Username)
                  .WithErrorMessage("使用者名稱已被使用");
            await this._mockUserService.Received(1).IsUsernameAvailableAsync("existinguser");
        }

        [Fact]
        public async Task ValidateAsync_電子郵件已註冊_應該驗證失敗()
        {
            // Arrange
            var request = this.CreateValidRequest();
            request.Email = "existing@example.com";

            this._mockUserService.IsUsernameAvailableAsync(Arg.Any<string>())
                .Returns(Task.FromResult(true));
            this._mockUserService.IsEmailRegisteredAsync("existing@example.com")
                .Returns(Task.FromResult(true));

            // Act
            var result = await this._validator.TestValidateAsync(request);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("此電子郵件已被註冊");
            await this._mockUserService.Received(1).IsEmailRegisteredAsync("existing@example.com");
        }

        [Fact]
        public async Task ValidateAsync_外部服務拋出例外_應該正確處理()
        {
            // Arrange
            var request = this.CreateValidRequest();
            request.Username = "testuser";

            this._mockUserService.IsUsernameAvailableAsync("testuser")
                .Returns(Task.FromException<bool>(new TimeoutException("服務逾時")));

            // Act & Assert
            var act = async () => await this._validator.TestValidateAsync(request);
            await act.Should().ThrowAsync<TimeoutException>()
                     .WithMessage("服務逾時");

            await this._mockUserService.Received(1).IsUsernameAvailableAsync("testuser");
        }

        private UserRegistrationRequest CreateValidRequest()
        {
            return new UserRegistrationRequest
            {
                Username = "testuser123",
                Email = "test@example.com",
                Password = "TestPass123",
                ConfirmPassword = "TestPass123",
                BirthDate = new DateTime(1990, 1, 1),
                Age = 34,
                PhoneNumber = "0912345678",
                Roles = ["User"],
                AgreeToTerms = true
            };
        }
    }
}