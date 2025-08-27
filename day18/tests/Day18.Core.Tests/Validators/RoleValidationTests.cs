namespace Day18.Core.Tests.Validators;

/// <summary>
/// 角色驗證測試
/// </summary>
public class RoleValidationTests
{
    private readonly UserRegistrationValidator _validator;

    public RoleValidationTests()
    {
        var fakeTimeProvider = new FakeTimeProvider();
        fakeTimeProvider.SetUtcNow(new DateTime(2024, 1, 1));
        this._validator = new UserRegistrationValidator(fakeTimeProvider);
    }

    #region Basic Role Validation

    [Fact]
    public void Validate_角色清單為null_應該產生驗證錯誤()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.Roles = null!;

        // Act
        var result = this._validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Roles)
              .WithErrorMessage("至少需要指定一個角色");
    }

    [Fact]
    public void Validate_角色清單為空_應該產生驗證錯誤()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.Roles = [];

        // Act
        var result = this._validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Roles)
              .WithErrorMessage("至少需要指定一個角色");
    }

    [Theory]
    [InlineData("User")]
    [InlineData("Admin")]
    [InlineData("Manager")]
    [InlineData("Support")]
    public void Validate_有效的角色_應該通過驗證(string role)
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.Roles = [role];

        // Act
        var result = this._validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Roles);
    }

    [Theory]
    [InlineData("InvalidRole")]
    [InlineData("SuperUser")]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_無效的角色_應該產生驗證錯誤(string invalidRole)
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.Roles = [invalidRole];

        // Act
        var result = this._validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Roles)
              .WithErrorMessage("包含無效的角色");
    }

    #endregion

    #region Multiple Roles Validation

    [Fact]
    public void Validate_多個有效角色_應該通過驗證()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.Roles = ["User", "Manager"];

        // Act
        var result = this._validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Roles);
    }

    [Fact]
    public void Validate_包含無效角色的混合清單_應該產生驗證錯誤()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.Roles = new List<string> { "User", "InvalidRole", "Admin" };

        // Act
        var result = this._validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Roles)
              .WithErrorMessage("包含無效的角色");
    }

    [Fact]
    public void Validate_所有有效角色_應該通過驗證()
    {
        // Arrange
        var request = this.CreateValidRequest();
        request.Roles = ["User", "Admin", "Manager", "Support"];

        // Act
        var result = this._validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Roles);
    }

    #endregion

    /// <summary>
    /// 建立有效的測試請求
    /// </summary>
    private UserRegistrationRequest CreateValidRequest()
    {
        return new UserRegistrationRequest
        {
            Username = "testuser123",
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!",
            BirthDate = new DateTime(1990, 1, 1),
            Age = 34,
            PhoneNumber = "0912345678",
            Roles = ["User"],
            AgreeToTerms = true
        };
    }
}