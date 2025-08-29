namespace Day20.Core.Integration.Tests.Integration;

public class UserServiceSqlServerTests : IAsyncLifetime
{
    private readonly MsSqlContainer _container;
    private UserDbContext _dbContext = null!;
    private SqlUserService _userService = null!;

    /// <summary>
    /// 建構式，初始化 SQL Server 容器
    /// </summary>
    public UserServiceSqlServerTests()
    {
        // 建立 SQL Server Testcontainer
        _container = new MsSqlBuilder()
                     .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                     .WithPassword("TestPass123!")
                     .WithPortBinding(15433, true)
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
                      .UseSqlServer(_container.GetConnectionString())
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
            Username = "testuser_sqlserver",
            FullName = "Test User_sqlserver",
            Email = "test_sqlserver@example.com",
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
            Username = "testuser2_sqlserver",
            FullName = "Test User2_sqlserver",
            Email = "test2_sqlserver@example.com",
            Age = 25
        };
        var createdUser = await _userService.CreateUserAsync(createRequest);

        // Act
        var result = await _userService.GetUserByIdAsync(createdUser.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(createdUser.Id);
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
            Username = "testuser3_sqlserver",
            FullName = "Test User3_sqlserver",
            Email = "test3_sqlserver@example.com",
            Age = 25
        };
        var request2 = new UserCreateRequest
        {
            Username = "testuser4_sqlserver",
            FullName = "Test User4_sqlserver",
            Email = "test4_sqlserver@example.com",
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
}