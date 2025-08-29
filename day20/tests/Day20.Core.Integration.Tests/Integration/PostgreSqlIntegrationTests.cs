namespace Day20.Core.Integration.Tests.Integration;

/// <summary>
/// PostgreSQL 整合測試
/// </summary>
public class PostgreSqlIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;
    private UserDbContext _context = null!;

    /// <summary>
    /// 建構式，初始化 PostgreSQL 容器
    /// </summary>
    public PostgreSqlIntegrationTests()
    {
        _postgres = new PostgreSqlBuilder()
                    .WithImage("postgres:15-alpine")
                    .WithDatabase("testdb")
                    .WithUsername("testuser")
                    .WithPassword("testpass")
                    .Build();
    }

    /// <summary>
    /// 初始化測試環境
    /// </summary>
    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        var options = new DbContextOptionsBuilder<UserDbContext>()
                      .UseNpgsql(_postgres.GetConnectionString())
                      .Options;

        _context = new UserDbContext(options);
        await _context.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// 清理測試環境
    /// </summary>
    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task CreateUser_應成功建立使用者()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "newtestuser",
            Email = "newtest@example.com",
            FullName = "New Test User",
            Age = 25
        };

        // Act
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Assert
        var savedUser = await _context.Users
                                      .FirstOrDefaultAsync(u => u.Username == "newtestuser");

        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be("newtest@example.com");
        savedUser.FullName.Should().Be("New Test User");
        savedUser.Age.Should().Be(25);
        savedUser.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetActiveUsers_應只回傳啟用的使用者()
    {
        // Arrange
        var users = new[]
        {
            new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "active1",
                Email = "active1@example.com",
                FullName = "Active User 1",
                Age = 30,
                IsActive = true
            },
            new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "inactive1",
                Email = "inactive1@example.com",
                FullName = "Inactive User 1",
                Age = 25,
                IsActive = false
            },
            new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "active2",
                Email = "active2@example.com",
                FullName = "Active User 2",
                Age = 35,
                IsActive = true
            }
        };

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var activeUsers = await _context.Users
                                        .Where(u => u.IsActive)
                                        .OrderBy(u => u.Username)
                                        .ToListAsync();

        // Assert - 預期有 4 個活躍使用者：種子資料中的 2 個 + 測試新增的 2 個
        activeUsers.Should().HaveCount(4);

        // 檢查測試新增的兩個使用者存在
        activeUsers.Should().ContainSingle(u => u.Username == "active1");
        activeUsers.Should().ContainSingle(u => u.Username == "active2");
    }
}