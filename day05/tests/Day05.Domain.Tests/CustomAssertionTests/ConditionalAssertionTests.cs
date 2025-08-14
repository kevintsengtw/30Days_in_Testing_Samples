using Day05.Domain.Services.UserServices;

namespace Day05.Domain.Tests.CustomAssertionTests;

// 使用條件式 Assertions
public class ConditionalAssertionTests
{
    private readonly UserService _userService;

    public ConditionalAssertionTests()
    {
        this._userService = new UserService();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ProcessUser_依據角色_應有正確權限(bool isAdmin)
    {
        // Act
        var actual = this._userService.ProcessUser(isAdmin);

        actual.Should().NotBeNull();

        // 這邊有個不好的示範，測試程式裡盡量避免使用 if 判斷式
        if (isAdmin)
        {
            actual.Role.Should().Be("admin");
            actual.Permissions.Should().Contain("DELETE");
        }
        else
        {
            actual.Role.Should().Be("user");
            actual.Permissions.Should().NotContain("DELETE");
        }
    }

    [Theory]
    [InlineData(true, "admin", true)]  // admin 應該有 DELETE 權限
    [InlineData(false, "user", false)] // user 不應該有 DELETE 權限
    public void ProcessUser_依據角色_應有正確權限_無if判斷式(bool isAdmin, string expectedRole, bool shouldHaveDeletePermission)
    {
        // Act
        var actual = this._userService.ProcessUser(isAdmin);

        actual.Should().NotBeNull();
        actual.Role.Should().Be(expectedRole);
        actual.Permissions.Contains("DELETE").Should().Be(shouldHaveDeletePermission);
    }
}