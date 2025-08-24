using AutoFixture;
using Bogus;
using Day15.Core.Models;
using Day15.TestLibrary.TestData.Extensions;

namespace Day15.TestLibrary.TestData.Factories;

/// <summary>
/// 整合測試資料工廠
/// </summary>
public class IntegratedTestDataFactory
{
    private readonly IFixture _fixture;
    private readonly Dictionary<Type, object> _cache;

    public IntegratedTestDataFactory(int? seed = null)
    {
        this._cache = new Dictionary<Type, object>();

        this._fixture = new Fixture().WithBogus()
                                     .WithOmitOnRecursion()
                                     .WithRepeatCount(3);

        if (seed.HasValue)
        {
            this._fixture.WithSeed(seed.Value);
        }

        // 初始化產生器
        this.InitializeGenerators();
    }

    /// <summary>
    /// 取得或建立快取版本的產生器
    /// </summary>
    public T GetCached<T>() where T : class
    {
        var type = typeof(T);
        if (this._cache.TryGetValue(type, out var cached))
        {
            return (T)cached;
        }

        var instance = this._fixture.Create<T>();
        this._cache[type] = instance;
        return instance;
    }

    /// <summary>
    /// 建立新的實例（不使用快取）
    /// </summary>
    public T CreateFresh<T>()
    {
        return this._fixture.Create<T>();
    }

    /// <summary>
    /// 建立多個實例
    /// </summary>
    public List<T> CreateMany<T>(int count = 3)
    {
        return this._fixture.CreateMany<T>(count).ToList();
    }

    /// <summary>
    /// 建立並設定實例
    /// </summary>
    public T Create<T>(Action<T> configure)
    {
        var instance = this._fixture.Create<T>();
        configure(instance);
        return instance;
    }

    /// <summary>
    /// 清除快取
    /// </summary>
    public void ClearCache()
    {
        this._cache.Clear();
    }

    /// <summary>
    /// 取得底層 AutoFixture 實例
    /// </summary>
    public IFixture GetFixture()
    {
        return this._fixture;
    }

    private void InitializeGenerators()
    {
        // 註冊特殊的 Faker，例如台灣地區相關資料
        var taiwanUserFaker = new Faker<User>("zh_TW").RuleFor(u => u.Id, f => f.Random.Guid())
                                                      .RuleFor(u => u.FirstName, f => f.Person.FirstName)
                                                      .RuleFor(u => u.LastName, f => f.Person.LastName)
                                                      .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                                                      .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber("09########"))
                                                      .RuleFor(u => u.BirthDate, f => f.Person.DateOfBirth)
                                                      .RuleFor(u => u.Age, f => f.Random.Int(18, 80));

        // 可以選擇性地使用台灣地區的 Faker
        // _fixture.WithBogusFor(taiwanUserFaker);
    }

    /// <summary>
    /// 建立完整的測試場景
    /// </summary>
    public TestScenario CreateTestScenario()
    {
        var company = this.CreateFresh<Company>();
        var users = this.CreateMany<User>(5);
        var orders = this.CreateMany<Order>(10);

        // 建立關聯
        foreach (var user in users)
        {
            user.Company = company;
            user.HomeAddress = this.CreateFresh<Address>();
        }

        foreach (var order in orders)
        {
            order.Customer = users[Random.Shared.Next(users.Count)];
            order.Items = this.CreateMany<OrderItem>(Random.Shared.Next(1, 5));

            foreach (var item in order.Items)
            {
                item.Product = this.CreateFresh<Product>();
            }

            order.TotalAmount = order.Items.Sum(i => i.TotalPrice);
        }

        company.Employees = users;

        return new TestScenario
        {
            Company = company,
            Users = users,
            Orders = orders
        };
    }
}

/// <summary>
/// class TestScenario - 測試場景資料結構
/// </summary>
public class TestScenario
{
    /// <summary>
    /// 公司
    /// </summary>
    public Company Company { get; set; } = new();
    
    /// <summary>
    /// 使用者列表
    /// </summary>
    public List<User> Users { get; set; } = new();
    
    /// <summary>
    /// 訂單列表
    /// </summary>
    public List<Order> Orders { get; set; } = new();
}