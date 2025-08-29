namespace Day20.Core.Integration.Tests.Integration;

public class UserServicePostgreSqlTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    private UserDbContext _dbContext = null!;
    private SqlUserService _userService = null!;

    /// <summary>
    /// 建構式，初始化 PostgreSQL 容器
    /// </summary>
    public UserServicePostgreSqlTests()
    {
        // 建立 PostgreSQL Testcontainer
        _container = new PostgreSqlBuilder()
                     .WithImage("postgres:15-alpine")
                     .WithDatabase("testdb")
                     .WithUsername("testuser")
                     .WithPassword("testpass")
                     .WithPortBinding(54321, true)
                     .Build();
    }

    /// <summary>
    /// 初始化測試環境
    /// </summary>
    public async Task InitializeAsync()
    {
        // 啟動容器
        await _container.StartAsync();

        // 設定 DbContext
        var options = new DbContextOptionsBuilder<UserDbContext>()
                      .UseNpgsql(_container.GetConnectionString())
                      .Options;

        _dbContext = new UserDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        _userService = new SqlUserService(_dbContext);
    }

    /// <summary>
    /// 清理測試環境
    /// </summary>
    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _container.DisposeAsync();
    }

    [Fact]
    public async Task CreateUserAsync_輸入有效使用者資料_應成功建立使用者()
    {
        // Arrange
        var request = new UserCreateRequest
        {
            Username = "testuser_postgres",
            FullName = "Test User_postgres",
            Email = "test_postgres@example.com",
            Age = 25
        };

        // Act
        var result = await _userService.CreateUserAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
        result.Username.Should().Be(request.Username);
        result.FullName.Should().Be(request.FullName);
        result.Email.Should().Be(request.Email);
        result.Age.Should().Be(request.Age);
    }

    [Fact]
    public async Task GetUserByIdAsync_輸入已存在的使用者ID_應回傳對應使用者()
    {
        // Arrange
        var createRequest = new UserCreateRequest
        {
            Username = "testuser2_postgres",
            FullName = "Test User2_postgres",
            Email = "test2_postgres@example.com",
            Age = 25
        };
        var createdUser = await _userService.CreateUserAsync(createRequest);

        // Act
        var result = await _userService.GetUserByIdAsync(createdUser.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdUser.Id);
        result.Username.Should().Be(createRequest.Username);
        result.FullName.Should().Be(createRequest.FullName);
        result.Email.Should().Be(createRequest.Email);
    }

    [Fact]
    public async Task GetAllUsersAsync_建立多個使用者後_應回傳所有使用者()
    {
        // Arrange
        var request1 = new UserCreateRequest
        {
            Username = "testuser3_postgres",
            FullName = "Test User3_postgres",
            Email = "test3_postgres@example.com",
            Age = 25
        };
        var request2 = new UserCreateRequest
        {
            Username = "testuser4_postgres",
            FullName = "Test User4_postgres",
            Email = "test4_postgres@example.com",
            Age = 30
        };

        await _userService.CreateUserAsync(request1);
        await _userService.CreateUserAsync(request2);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        result.Count().Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task Database_檢查資料庫連線_應能成功連接()
    {
        // Arrange & Act
        var canConnect = await _dbContext.Database.CanConnectAsync();

        // Assert
        canConnect.Should().BeTrue();
    }

    [Fact]
    public async Task CreateUser_應建立新使用者()
    {
        // Arrange
        var username = "newuser1";
        var email = "newuser1@example.com";

        // Act
        var user = await _userService.CreateUserAsync(username, email);

        // Assert
        user.Should().NotBeNull();
        user.Username.Should().Be(username);
        user.Email.Should().Be(email);
        user.Id.Should().NotBeNullOrEmpty();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateUser_當使用者名稱重複_應拋出異常()
    {
        // Arrange
        var username = "duplicateuser";
        await _userService.CreateUserAsync(username, "first@example.com");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.CreateUserAsync(username, "second@example.com"));

        exception.Message.Should().Contain($"使用者名稱 '{username}' 已存在");
    }

    [Fact]
    public async Task GetUserByUsername_當使用者存在_應回傳使用者()
    {
        // Arrange
        var username = "newuser2";
        var email = "newuser2@example.com";
        await _userService.CreateUserAsync(username, email);

        // Act
        var user = await _userService.GetUserByUsernameAsync(username);

        // Assert
        user.Should().NotBeNull();
        user!.Username.Should().Be(username);
        user.Email.Should().Be(email);
    }

    [Fact]
    public async Task GetUserByUsername_當使用者不存在_應回傳Null()
    {
        // Act
        var user = await _userService.GetUserByUsernameAsync("nonexistentuser");

        // Assert
        user.Should().BeNull();
    }

    [Fact]
    public async Task GetActiveUsers_應只回傳啟用的使用者()
    {
        // Arrange
        await _userService.CreateUserAsync("active3", "active3@example.com");
        await _userService.CreateUserAsync("active4", "active4@example.com");

        // 創建一個非啟用的使用者
        var inactiveUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "inactive1",
            Email = "inactive1@example.com",
            FullName = "Inactive User",
            Age = 25,
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Users.Add(inactiveUser);
        await _dbContext.SaveChangesAsync();

        // Act
        var activeUsers = await _userService.GetActiveUsersAsync();

        // Assert
        // 種子資料中有 2 個啟用使用者 + 我們新增的 2 個 = 4 個啟用使用者
        activeUsers.Should().HaveCount(4);
        activeUsers.Should().OnlyContain(u => u.IsActive);
        activeUsers.Should().BeInAscendingOrder(u => u.Username);
    }
}