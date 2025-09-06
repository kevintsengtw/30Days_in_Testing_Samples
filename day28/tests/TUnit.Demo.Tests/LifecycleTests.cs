namespace TUnit.Demo.Tests;

/// <summary>
/// 展示基本生命週期管理的測試類別
/// </summary>
public class BasicLifecycleTests : IDisposable
{
    private readonly Calculator _calculator;

    // 每個測試執行前都會呼叫建構式
    public BasicLifecycleTests()
    {
        _calculator = new Calculator();
        Console.WriteLine("建構式：建立 Calculator 實例");
    }

    [Test]
    public async Task Add_基本測試()
    {
        Console.WriteLine("執行測試：Add_基本測試");
        await Assert.That(_calculator.Add(1, 2)).IsEqualTo(3);
    }

    [Test]
    public async Task Multiply_基本測試()
    {
        Console.WriteLine("執行測試：Multiply_基本測試");
        await Assert.That(_calculator.Multiply(3, 4)).IsEqualTo(12);
    }

    // 每個測試執行後都會呼叫 Dispose
    public void Dispose()
    {
        Console.WriteLine("Dispose：清理資源");
        // 進行必要的清理工作
    }
}

/// <summary>
/// 展示完整生命週期管理的測試類別
/// </summary>
public class DatabaseLifecycleTests
{
    private static TestDatabase? _database;

    // 所有測試執行前只執行一次
    [Before(Class)]
    public static async Task ClassSetup()
    {
        _database = new TestDatabase();
        await _database.InitializeAsync();
        Console.WriteLine("資料庫初始化完成");
    }

    // 每個測試執行前都會執行
    [Before(Test)]
    public async Task TestSetup()
    {
        Console.WriteLine("測試準備：清理資料庫狀態");
        await _database!.ClearDataAsync();
    }

    [Test]
    public async Task 測試使用者建立()
    {
        // Arrange
        var userService = new UserService(_database!);

        // Act
        var user = await userService.CreateUserAsync("test@example.com");

        // Assert
        await Assert.That(user.Id).IsNotEqualTo(Guid.Empty);
        await Assert.That(user.Email).IsEqualTo("test@example.com");
    }

    [Test]
    public async Task 測試使用者查詢()
    {
        // Arrange
        var userService = new UserService(_database!);
        await userService.CreateUserAsync("query@example.com");

        // Act
        var user = await userService.GetUserByEmailAsync("query@example.com");

        // Assert
        await Assert.That(user).IsNotNull();
        await Assert.That(user!.Email).IsEqualTo("query@example.com");
    }

    // 每個測試執行後都會執行
    [After(Test)]
    public async Task TestTearDown()
    {
        Console.WriteLine("測試清理：記錄測試結果");
        // 可以在這裡記錄測試執行資訊
        await Task.CompletedTask;
    }

    // 所有測試執行後只執行一次
    [After(Class)]
    public static async Task ClassTearDown()
    {
        if (_database != null)
        {
            await _database.DisposeAsync();
            Console.WriteLine("資料庫連線關閉");
        }
    }
}

/// <summary>
/// 展示生命週期執行順序的測試類別
/// </summary>
public class LifecycleOrderDemoTests : IDisposable
{
    public LifecycleOrderDemoTests()
    {
        Console.WriteLine("2. 建構式執行");
    }

    [Before(Class)]
    public static void ClassSetup()
    {
        Console.WriteLine("1. Before(Class) 執行");
    }

    [Before(Test)]
    public async Task TestSetup()
    {
        Console.WriteLine("3. Before(Test) 執行");
        await Task.CompletedTask;
    }

    [Test]
    public async Task 示範測試()
    {
        Console.WriteLine("4. 測試方法執行");
        var result = 1 + 1 == 2;
        await Assert.That(result).IsTrue();
    }

    [After(Test)]
    public async Task TestTearDown()
    {
        Console.WriteLine("5. After(Test) 執行");
        await Task.CompletedTask;
    }

    public void Dispose()
    {
        Console.WriteLine("6. Dispose 執行");
    }

    [After(Class)]
    public static void ClassTearDown()
    {
        Console.WriteLine("7. After(Class) 執行");
    }
}

/// <summary>
/// 模擬的測試資料庫類別
/// </summary>
public class TestDatabase : IAsyncDisposable
{
    private readonly Dictionary<string, User> _users = new();

    public async Task InitializeAsync()
    {
        // 模擬資料庫初始化
        await Task.Delay(50);
    }

    public async Task ClearDataAsync()
    {
        _users.Clear();
        await Task.Delay(10);
    }

    public async Task<User> CreateUserAsync(string email)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            CreatedAt = DateTime.Now
        };
        _users[email] = user;
        await Task.Delay(10);
        return user;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        await Task.Delay(10);
        return _users.GetValueOrDefault(email);
    }

    public async ValueTask DisposeAsync()
    {
        _users.Clear();
        await Task.Delay(10);
    }
}

/// <summary>
/// 模擬的使用者服務類別
/// </summary>
public class UserService
{
    private readonly TestDatabase _database;

    public UserService(TestDatabase database)
    {
        _database = database;
    }

    public async Task<User> CreateUserAsync(string email)
    {
        return await _database.CreateUserAsync(email);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _database.GetUserByEmailAsync(email);
    }
}