/// <summary>
/// 修復後的穩定使用者服務測試
/// 展示如何正確處理測試中的常見問題
/// </summary>
[Trait("Category", "Fixed")]
public class StableUserServiceTests
{
    private readonly IUserRepository _userRepository;

    public StableUserServiceTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
    }

    [Fact]
    public void GetActiveUsers_有活躍和非活躍使用者_應只回傳活躍使用者()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = "1", Name = "活躍使用者1", IsActive = true },
            new() { Id = "2", Name = "非活躍使用者", IsActive = false },
            new() { Id = "3", Name = "活躍使用者2", IsActive = true }
        };

        _userRepository.GetAllUsersAsync().Returns(users);
        var userService = new UserService(_userRepository);

        // Act
        var result = userService.GetActiveUsersAsync().Result;

        // Assert
        result.Should().HaveCount(2);
        result.All(u => u.IsActive).Should().BeTrue();
        result.Select(u => u.Id).Should().Contain(new[] { "1", "3" });
    }

    [Fact]
    public void ValidateUser_存在的活躍使用者_應回傳True()
    {
        // Arrange
        var user = new User { Id = "test-user", IsActive = true };
        _userRepository.GetUserByIdAsync("test-user").Returns(user);
        var userService = new UserService(_userRepository);

        // Act
        var result = userService.ValidateUserAsync("test-user").Result;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateUser_不存在的使用者_應回傳False()
    {
        // Arrange
        _userRepository.GetUserByIdAsync("non-existent").Returns((User?)null);
        var userService = new UserService(_userRepository);

        // Act
        var result = userService.ValidateUserAsync("non-existent").Result;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateUser_空的使用者ID_應回傳False()
    {
        // Arrange
        var userService = new UserService(_userRepository);

        // Act
        var result1 = userService.ValidateUserAsync("").Result;
        var result2 = userService.ValidateUserAsync(null!).Result;

        // Assert
        result1.Should().BeFalse();
        result2.Should().BeFalse();
    }

    [Fact]
    public void ValidateUser_非活躍使用者_應回傳False()
    {
        // Arrange
        var user = new User { Id = "inactive-user", IsActive = false };
        _userRepository.GetUserByIdAsync("inactive-user").Returns(user);
        var userService = new UserService(_userRepository);

        // Act
        var result = userService.ValidateUserAsync("inactive-user").Result;

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("user1", true)]
    [InlineData("user2", false)]
    [InlineData("user3", true)]
    public void ValidateUser_多種使用者狀態_應回傳對應結果(string userId, bool isActive)
    {
        // Arrange
        var user = new User { Id = userId, IsActive = isActive };
        _userRepository.GetUserByIdAsync(userId).Returns(user);
        var userService = new UserService(_userRepository);

        // Act
        var result = userService.ValidateUserAsync(userId).Result;

        // Assert
        result.Should().Be(isActive);
    }
}
