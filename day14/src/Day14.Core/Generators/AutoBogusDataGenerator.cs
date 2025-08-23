using Day14.Core.Models;

namespace Day14.Core.Generators;

/// <summary>
/// AutoFixture 和 AutoBogus 資料產生器
/// </summary>
public static class AutoBogusDataGenerator
{
    /// <summary>
    /// 建立 AutoFixture 配置
    /// </summary>
    private static IFixture CreateFixture()
    {
        return new Fixture().Customize(new AutoNSubstituteCustomization());
    }

    /// <summary>
    /// 使用 AutoFixture 產生產品
    /// </summary>
    public static Product CreateProductWithAutoFixture()
    {
        var fixture = CreateFixture();
        return fixture.Create<Product>();
    }

    /// <summary>
    /// 使用 AutoFixture 產生使用者
    /// </summary>
    public static User CreateUserWithAutoFixture()
    {
        var fixture = CreateFixture();
        return fixture.Create<User>();
    }

    /// <summary>
    /// 使用 AutoFixture 產生台灣人
    /// </summary>
    public static TaiwanPerson CreateTaiwanPersonWithAutoFixture()
    {
        var fixture = CreateFixture();
        return fixture.Create<TaiwanPerson>();
    }

    /// <summary>
    /// 使用 AutoFixture 產生訂單
    /// </summary>
    public static Order CreateOrderWithAutoFixture()
    {
        var fixture = CreateFixture();
        return fixture.Create<Order>();
    }

    /// <summary>
    /// 使用 AutoFixture 產生員工
    /// </summary>
    public static Employee CreateEmployeeWithAutoFixture()
    {
        var fixture = CreateFixture();
        return fixture.Create<Employee>();
    }

    /// <summary>
    /// 使用 AutoFixture 產生多個產品
    /// </summary>
    public static List<Product> CreateMultipleProducts(int count)
    {
        var fixture = CreateFixture();
        return fixture.CreateMany<Product>(count).ToList();
    }

    /// <summary>
    /// 使用 AutoFixture 產生多個使用者
    /// </summary>
    public static List<User> CreateMultipleUsers(int count)
    {
        var fixture = CreateFixture();
        return fixture.CreateMany<User>(count).ToList();
    }

    /// <summary>
    /// 使用 AutoFixture 產生多個台灣人
    /// </summary>
    public static List<TaiwanPerson> CreateMultipleTaiwanPeople(int count)
    {
        var fixture = CreateFixture();
        return fixture.CreateMany<TaiwanPerson>(count).ToList();
    }

    /// <summary>
    /// 使用 AutoFixture 產生多個訂單
    /// </summary>
    public static List<Order> CreateMultipleOrders(int count)
    {
        var fixture = CreateFixture();
        return fixture.CreateMany<Order>(count).ToList();
    }

    /// <summary>
    /// 使用 AutoFixture 產生多個員工
    /// </summary>
    public static List<Employee> CreateMultipleEmployees(int count)
    {
        var fixture = CreateFixture();
        return fixture.CreateMany<Employee>(count).ToList();
    }

    /// <summary>
    /// 使用 AutoFixture 建立客製化產品
    /// </summary>
    public static Product CreateCustomProduct(string name, decimal price)
    {
        var fixture = CreateFixture();
        return fixture.Build<Product>()
                      .With(p => p.Name, name)
                      .With(p => p.Price, price)
                      .Create();
    }

    /// <summary>
    /// 使用 AutoFixture 建立客製化使用者
    /// </summary>
    public static User CreateCustomUser(string email, int age)
    {
        var fixture = CreateFixture();
        return fixture.Build<User>()
                      .With(u => u.Email, email)
                      .With(u => u.Age, age)
                      .Create();
    }
}