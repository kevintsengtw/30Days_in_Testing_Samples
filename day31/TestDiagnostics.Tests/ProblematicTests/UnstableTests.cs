/// <summary>
/// 故意設計的不穩定測試案例
/// 這些測試展示了常見的測試問題，用於診斷和修復練習
/// </summary>
[Trait("Category", "Problematic")]
public class UnstableTests
{
    private static int _sharedCounter = 0; // 靜態狀態污染
    private readonly IUserRepository _userRepository;

    public UnstableTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
    }

    [Fact]
    public void GetUserCount_使用靜態計數器_可能因執行順序失敗()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = "1", Name = "使用者1", IsActive = true },
            new() { Id = "2", Name = "使用者2", IsActive = true }
        };

        _userRepository.GetAllUsersAsync().Returns(users);
        var userService = new UserService(_userRepository);

        // Act
        var result = userService.GetUserCountAsync().Result;

        // Assert
        // 這個測試會因為靜態狀態而不穩定
        // 第一次執行可能通過，後續執行可能失敗
        result.Should().Be(2);

        // 靜態計數器會保持狀態，造成測試間的相互影響
        _sharedCounter++;
    }

    [Fact]
    public void GetUserCount_重複執行_靜態狀態造成不一致結果()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = "1", Name = "使用者1", IsActive = true }
        };

        _userRepository.GetAllUsersAsync().Returns(users);
        var userService = new UserService(_userRepository);

        // Act
        var result1 = userService.GetUserCountAsync().Result;
        var result2 = userService.GetUserCountAsync().Result;

        // Assert
        // 由於 UserService 內部使用靜態計數器，
        // 這個測試會在多次執行時產生不同的結果
        result1.Should().Be(1);
        result2.Should().Be(1); // 這可能會失敗
    }

    [Fact]
    public void ValidateLogin_使用當前時間_結果不可預測()
    {
        // Arrange
        var user = new User
        {
            Id = "test-user",
            IsActive = true,
            LastLoginTime = DateTime.Now.AddMinutes(-30) // 使用當前時間會造成不確定性
        };

        _userRepository.GetUserByIdAsync("test-user").Returns(user);
        var userService = new UserService(_userRepository);

        // Act
        var result = userService.IsUserLoginValidAsync("test-user").Result;

        // Assert
        // 這個測試的結果取決於執行時間，不可預測
        result.Should().BeTrue();
    }

    [Fact]
    public void ProcessMultipleUsers_競爭條件_間歇性失敗()
    {
        // Arrange
        var users = new List<User>();
        for (int i = 0; i < 100; i++)
        {
            users.Add(new User { Id = $"user-{i}", IsActive = i % 2 == 0 });
        }

        _userRepository.GetAllUsersAsync().Returns(users);
        var userService = new UserService(_userRepository);

        // Act & Assert
        // 模擬並行處理，可能造成間歇性失敗
        var tasks = new List<Task<List<User>>>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(userService.GetActiveUsersAsync());
        }

        var results = Task.WhenAll(tasks).Result;

        // 這個斷言可能會因為內部狀態而間歇性失敗
        results.All(r => r.Count == 50).Should().BeTrue();
    }
}
