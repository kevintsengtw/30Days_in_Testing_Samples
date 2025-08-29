namespace Day20.Core.Integration.Tests.Integration;

/// <summary>
/// SQL Server 整合測試
/// </summary>
public class SqlServerIntegrationTests : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServer;
    private UserDbContext _context = null!;

    /// <summary>
    /// 建構式，初始化 SQL Server 容器
    /// </summary>
    public SqlServerIntegrationTests()
    {
        _sqlServer = new MsSqlBuilder()
                     .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                     .WithPassword("YourStrong@Passw0rd")
                     .WithEnvironment("ACCEPT_EULA", "Y")
                     .Build();
    }

    /// <summary>
    /// 初始化測試環境
    /// </summary>
    public async Task InitializeAsync()
    {
        await _sqlServer.StartAsync();

        var options = new DbContextOptionsBuilder<UserDbContext>()
                      .UseSqlServer(_sqlServer.GetConnectionString())
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
        await _sqlServer.DisposeAsync();
    }

    [Fact]
    public async Task CreateUser_在SQLServer中_應成功建立使用者()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "sqlserveruser",
            Email = "sqlserver@example.com",
            FullName = "SQL Server User",
            Age = 28
        };

        // Act
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Assert
        var savedUser = await _context.Users
                                      .FirstOrDefaultAsync(u => u.Username == "sqlserveruser");

        savedUser.Should().NotBeNull();
        savedUser!.Email.Should().Be("sqlserver@example.com");
        savedUser.FullName.Should().Be("SQL Server User");
        savedUser.Age.Should().Be(28);
        savedUser.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task UniqueConstraint_重複Email_應拋出例外()
    {
        // Arrange
        var user1 = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "user1",
            Email = "duplicate@example.com",
            FullName = "User One",
            Age = 25
        };

        var user2 = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "user2",
            Email = "duplicate@example.com",
            FullName = "User Two",
            Age = 30
        };

        // Act & Assert
        _context.Users.Add(user1);
        await _context.SaveChangesAsync();

        _context.Users.Add(user2);

        var exception = await Assert.ThrowsAsync<DbUpdateException>(
            () => _context.SaveChangesAsync());

        exception.Should().NotBeNull();
        exception.InnerException.Should().NotBeNull();
    }

    [Fact]
    public async Task Transaction_新增資料後於驗證有新增之後執行Rollback_應正確運作()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = "transactiontest",
            Email = "transaction@example.com",
            FullName = "Transaction Test",
            Age = 32
        };

        // Act
        await using var transaction = await _context.Database.BeginTransactionAsync();

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // 驗證資料存在
        var userCount = await _context.Users.CountAsync();
        userCount.Should().BeGreaterThan(0);

        // Rollback
        await transaction.RollbackAsync();

        // Assert - 建立新的 context 來驗證交易確實 Rollback
        var verifyOptions = new DbContextOptionsBuilder<UserDbContext>()
                            .UseSqlServer(_sqlServer.GetConnectionString())
                            .Options;

        await using var verifyContext = new UserDbContext(verifyOptions);
        var finalUserCount = await verifyContext.Users.CountAsync(u => u.Username == "transactiontest");

        finalUserCount.Should().Be(0);
    }
}